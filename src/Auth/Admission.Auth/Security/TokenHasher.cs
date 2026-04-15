using System.Security.Cryptography;
using System.Text;
using Admission.Auth.Options;
using Microsoft.Extensions.Options;

namespace Admission.Auth.Security;

public sealed class TokenHasher(IOptions<SecurityOptions> options) : ITokenHasher
{
    private readonly SecurityOptions _options = options.Value;

    public string Hash(string token)
    {
        var input = $"{token}:{_options.TokenPepper}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
