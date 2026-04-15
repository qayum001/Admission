using System.Security.Cryptography;
using Admission.Auth.Domain.Entities;
using Admission.Auth.Options;
using Admission.Auth.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Admission.Auth.Security.Signing;

public sealed class SigningKeyLifecycleService(
    AuthDbContext dbContext,
    IOptions<SigningKeyOptions> signingKeyOptions,
    ILogger<SigningKeyLifecycleService> logger) : ISigningKeyLifecycleService
{
    private readonly SigningKeyOptions _options = signingKeyOptions.Value;

    public async Task<SigningKeyState> EnsureAndLoadAsync(CancellationToken cancellationToken = default)
    {
        await EnsureActiveKeyAsync(cancellationToken);
        return await LoadStateAsync(cancellationToken);
    }

    private async Task EnsureActiveKeyAsync(CancellationToken cancellationToken)
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var active = await dbContext.SigningKeys.SingleOrDefaultAsync(x => x.IsActive, cancellationToken);
        var rotateAfter = TimeSpan.FromDays(_options.RotateAfterDays);

        if (active is null)
        {
            dbContext.SigningKeys.Add(CreateNewSigningKey(nowUtc));
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created initial JWT signing key");
            return;
        }

        if (active.ActivatedAt + rotateAfter <= nowUtc)
        {
            active.IsActive = false;
            active.RetiredAt = nowUtc;

            dbContext.SigningKeys.Add(CreateNewSigningKey(nowUtc));
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Rotated JWT signing key. Previous kid: {Kid}", active.Kid);
        }

        var cleanupBefore = nowUtc.AddDays(-_options.RetainDays);
        var oldKeys = await dbContext.SigningKeys
            .Where(x => !x.IsActive && x.RetiredAt != null && x.RetiredAt < cleanupBefore)
            .ToListAsync(cancellationToken);

        if (oldKeys.Count > 0)
        {
            dbContext.SigningKeys.RemoveRange(oldKeys);
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Deleted {Count} retired signing keys", oldKeys.Count);
        }
    }

    private async Task<SigningKeyState> LoadStateAsync(CancellationToken cancellationToken)
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var retainFrom = nowUtc.AddDays(-_options.RetainDays);

        var keyRecords = await dbContext.SigningKeys
            .Where(x => x.IsActive || (x.RetiredAt != null && x.RetiredAt >= retainFrom))
            .OrderByDescending(x => x.IsActive)
            .ThenByDescending(x => x.ActivatedAt)
            .ToListAsync(cancellationToken);

        var active = keyRecords.FirstOrDefault(x => x.IsActive)
            ?? throw new InvalidOperationException("No active signing key found.");

        var activeSigningKey = CreateSecurityKey(active, includePrivateKey: true);
        var activeSigningCredentials = new SigningCredentials(activeSigningKey, SecurityAlgorithms.RsaSha256);

        var validationKeys = keyRecords
            .Select(x => (SecurityKey)CreateSecurityKey(x, includePrivateKey: false))
            .ToList();

        var jwksKeys = keyRecords
            .Select(ToJwksKey)
            .ToList();

        return new SigningKeyState(activeSigningCredentials, validationKeys, jwksKeys);
    }

    private SigningKeyRecord CreateNewSigningKey(DateTimeOffset nowUtc)
    {
        using var rsa = RSA.Create(_options.RsaKeySize);

        return new SigningKeyRecord
        {
            Kid = $"auth-key-{Guid.NewGuid():N}",
            PrivateKeyPem = rsa.ExportPkcs8PrivateKeyPem(),
            CreatedAt = nowUtc,
            ActivatedAt = nowUtc,
            IsActive = true
        };
    }

    private static RsaSecurityKey CreateSecurityKey(SigningKeyRecord record, bool includePrivateKey)
    {
        using var sourceRsa = RSA.Create();
        sourceRsa.ImportFromPem(record.PrivateKeyPem);

        var rsa = RSA.Create();
        rsa.ImportParameters(sourceRsa.ExportParameters(includePrivateKey));

        return new RsaSecurityKey(rsa)
        {
            KeyId = record.Kid
        };
    }

    private static JwksKey ToJwksKey(SigningKeyRecord record)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(record.PrivateKeyPem);

        var parameters = rsa.ExportParameters(false);

        return new JwksKey(
            "RSA",
            "sig",
            "RS256",
            record.Kid,
            Base64UrlEncoder.Encode(parameters.Modulus!),
            Base64UrlEncoder.Encode(parameters.Exponent!));
    }
}
