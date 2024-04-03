namespace ScooterRental.Exceptions;

public class InvalidIdException : Exception
{
    public InvalidIdException() : base("Invalid ID.") { }
}