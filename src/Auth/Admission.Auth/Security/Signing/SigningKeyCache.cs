using Microsoft.IdentityModel.Tokens;

namespace Admission.Auth.Security.Signing;

public sealed class SigningKeyCache : ISigningKeyCache
{
    private readonly object _lock = new();
    private SigningCredentials? _activeSigningCredentials;
    private IReadOnlyCollection<SecurityKey> _validationKeys = [];
    private IReadOnlyCollection<JwksKey> _jwksKeys = [];

    public void Set(SigningKeyState state)
    {
        lock (_lock)
        {
            _activeSigningCredentials = state.ActiveSigningCredentials;
            _validationKeys = state.ValidationKeys;
            _jwksKeys = state.JwksKeys;
        }
    }

    public SigningCredentials GetActiveSigningCredentials()
    {
        lock (_lock)
        {
            return _activeSigningCredentials
                ?? throw new InvalidOperationException("Signing key cache is not initialized.");
        }
    }

    public IReadOnlyCollection<SecurityKey> GetValidationKeys()
    {
        lock (_lock)
        {
            return _validationKeys;
        }
    }

    public IReadOnlyCollection<JwksKey> GetJwksKeys()
    {
        lock (_lock)
        {
            return _jwksKeys;
        }
    }
}
