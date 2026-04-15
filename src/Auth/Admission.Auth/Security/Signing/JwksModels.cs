namespace Admission.Auth.Security.Signing;

public sealed record JwksKey(string Kty, string Use, string Alg, string Kid, string N, string E);

public sealed record JwksDocument(IReadOnlyCollection<JwksKey> Keys);
