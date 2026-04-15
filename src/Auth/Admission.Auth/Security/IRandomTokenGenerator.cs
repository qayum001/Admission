namespace Admission.Auth.Security;

public interface IRandomTokenGenerator
{
    string Generate(int byteLength = 64);
}
