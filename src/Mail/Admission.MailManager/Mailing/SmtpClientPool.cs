using System.Threading.Channels;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;

namespace Admission.MailManager.Mailing;

public sealed class SmtpClientPool(
    IOptions<SmtpOptions> options,
    ILogger<SmtpClientPool> logger) : ISmtpClientPool, IHostedService, IAsyncDisposable
{
    private readonly SmtpOptions _options = options.Value;
    private readonly Channel<SmtpClient> _clients = Channel.CreateBounded<SmtpClient>(
        new BoundedChannelOptions(Math.Max(1, options.Value.PoolSize))
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });

    private volatile bool _isStopping;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var poolSize = Math.Max(1, _options.PoolSize);

        for (var i = 0; i < poolSize; i++)
        {
            var client = await CreateConnectedClientAsync(cancellationToken);
            await _clients.Writer.WriteAsync(client, cancellationToken);
        }

        logger.LogInformation("Initialized SMTP client pool with {PoolSize} clients", poolSize);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _isStopping = true;
        _clients.Writer.TryComplete();

        logger.LogInformation("Stopping SMTP client pool");

        return Task.CompletedTask;
    }

    public async ValueTask<SmtpClientLease> RentAsync(CancellationToken cancellationToken = default)
    {
        var client = await _clients.Reader.ReadAsync(cancellationToken);

        if (client.IsConnected) return new SmtpClientLease(client, this);
        logger.LogWarning("SMTP client was disconnected while renting, creating a replacement");

        await DisconnectAndDisposeAsync(client);
        client = await CreateConnectedClientAsync(cancellationToken);

        return new SmtpClientLease(client, this);
    }

    internal async ValueTask ReturnAsync(SmtpClient client)
    {
        if (_isStopping || !_clients.Writer.TryWrite(client))
        {
            await DisconnectAndDisposeAsync(client);
            return;
        }

        logger.LogDebug("Returned SMTP client to pool");
    }

    internal async ValueTask ReplaceInvalidClientAsync(SmtpClient client)
    {
        await DisconnectAndDisposeAsync(client);

        if (_isStopping)
        {
            return;
        }

        var replacement = await CreateConnectedClientAsync(CancellationToken.None);

        if (!_clients.Writer.TryWrite(replacement))
        {
            await DisconnectAndDisposeAsync(replacement);
            return;
        }

        logger.LogWarning("Replaced invalid SMTP client in pool");
    }

    public async ValueTask DisposeAsync()
    {
        while (await _clients.Reader.WaitToReadAsync())
        {
            while (_clients.Reader.TryRead(out var client))
            {
                await DisconnectAndDisposeAsync(client);
            }
        }
    }

    private async Task<SmtpClient> CreateConnectedClientAsync(CancellationToken cancellationToken)
    {
        var client = new SmtpClient();

        await client.ConnectAsync(_options.Host, _options.Port, false, cancellationToken);

        logger.LogDebug("Created and connected SMTP client");

        return client;
    }

    private static async Task DisconnectAndDisposeAsync(SmtpClient client)
    {
        try
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }
        }
        finally
        {
            client.Dispose();
        }
    }
}
