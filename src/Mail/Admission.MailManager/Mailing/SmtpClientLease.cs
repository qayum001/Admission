using MailKit.Net.Smtp;

namespace Admission.MailManager.Mailing;

public sealed class SmtpClientLease : IAsyncDisposable
{
    private readonly SmtpClientPool _pool;
    private bool _disposed;
    private bool _isInvalid;

    internal SmtpClientLease(SmtpClient client, SmtpClientPool pool)
    {
        Client = client;
        _pool = pool;
    }

    public SmtpClient Client { get; }

    public void Invalidate()
    {
        _isInvalid = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_isInvalid || !Client.IsConnected)
        {
            await _pool.ReplaceInvalidClientAsync(Client);
            return;
        }

        await _pool.ReturnAsync(Client);
    }
}
