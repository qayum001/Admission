using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Admission.Auth.Security;

public sealed class RandomTokenGenerator : IRandomTokenGenerator
{
    public string Generate(int byteLength = 64)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteLength);
        return Base64UrlEncoder.Encode(bytes);
    }
}
