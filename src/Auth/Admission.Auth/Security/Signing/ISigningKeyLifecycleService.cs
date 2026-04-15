namespace Admission.Auth.Security.Signing;

public interface ISigningKeyLifecycleService
{
    Task<SigningKeyState> EnsureAndLoadAsync(CancellationToken cancellationToken = default);
}
