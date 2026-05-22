namespace Admission.Application.Exceptions;

public class InvalidActionException: Exception
{
    public InvalidActionException(string message) : base(message) { }
    public InvalidActionException() : base("Invalid action") { }
}