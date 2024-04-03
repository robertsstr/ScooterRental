namespace ScooterRental.Exceptions;

public class InvalidPriceException : Exception
{
    public InvalidPriceException() : base("Invalid scooter price.") { }
}