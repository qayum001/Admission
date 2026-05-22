using Admission.Domain.Exceptions;

namespace Admission.Domain.ValueObjects;

public class PhoneNumber
{
    public string Value { get; }
    
    public PhoneNumber(string val)
    {
        if (string.IsNullOrEmpty(val) || val.Length < 4)
            throw new InvalidDataDomainException("Phone number should be at least 4 characters");
        Value = val;
    }
}