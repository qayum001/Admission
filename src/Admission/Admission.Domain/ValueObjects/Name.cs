namespace Admission.Domain.ValueObjects;

public class Name
{
    public string Value { get; }

    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
            throw new ArgumentException("Value must be at least 2 characters long.", nameof(value));
        Value = value;
    }
}