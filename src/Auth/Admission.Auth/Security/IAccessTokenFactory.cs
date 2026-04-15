using Admission.Auth.Domain.Entities;

namespace Admission.Auth.Security;

public interface IAccessTokenFactory
{
    IssuedAccessToken Create(AuthUser user);
}
