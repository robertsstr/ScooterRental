namespace ScooterRental.Exceptions;

public class InvalidRentalDurationException : Exception
{
    public InvalidRentalDurationException() : base("Invalid Duration.") { }
}