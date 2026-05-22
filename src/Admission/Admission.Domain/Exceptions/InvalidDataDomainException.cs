namespace Admission.Domain.Exceptions;

public class InvalidDataDomainException(string message) : DomainException(message)
{
    public InvalidDataDomainException() : this("Invalid data") { }    
}