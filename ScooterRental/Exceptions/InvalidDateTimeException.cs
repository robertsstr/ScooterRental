namespace ScooterRental.Exceptions;

public class InvalidDateTimeException : Exception
{
    public InvalidDateTimeException() : base("Are you a time traveler?"){ }
}