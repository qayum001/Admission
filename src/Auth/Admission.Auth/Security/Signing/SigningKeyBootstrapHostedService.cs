namespace Admission.Auth.Security.Signing;

public sealed class SigningKeyBootstrapHostedService(
    IServiceScopeFactory scopeFactory,
    ISigningKeyCache signingKeyCache,
    ILogger<SigningKeyBootstrapHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var signingKeyLifecycleService = scope.ServiceProvider.GetRequiredService<ISigningKeyLifecycleService>();
        var state = await signingKeyLifecycleService.EnsureAndLoadAsync(cancellationToken);

        signingKeyCache.Set(state);

        logger.LogInformation("Signing key cache initialized with {Count} keys", state.ValidationKeys.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
