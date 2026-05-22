namespace Admission.Domain.ValueObjects;

public class Text
{
    public string Value { get; }
    public Text(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        Value = value;
    }
}