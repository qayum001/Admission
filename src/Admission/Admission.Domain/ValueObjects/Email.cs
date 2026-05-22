using System.Text.RegularExpressions;

namespace Admission.Domain.ValueObjects;

public class Email : IEquatable<Email>
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidDataException("Email cannot be null or whitespace");
        if (!IsEmail(value))
            throw new InvalidDataException("Invalid email address");
        Value = value;
    }

    private static bool IsEmail(string input)
    {
        const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(input, emailPattern, RegexOptions.IgnoreCase);
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        var other = (Email)obj;
        return other.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}