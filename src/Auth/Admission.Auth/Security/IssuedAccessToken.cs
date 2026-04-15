namespace Admission.Auth.Security;

public sealed record IssuedAccessToken(string Token, DateTimeOffset ExpiresAt, int ExpiresInSeconds);
