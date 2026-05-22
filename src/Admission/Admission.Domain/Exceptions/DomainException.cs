namespace Admission.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message){}
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
    public DomainException() : base("Something went wrong while operating in domain layer") { }
}