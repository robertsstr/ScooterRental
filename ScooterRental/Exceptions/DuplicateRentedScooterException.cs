namespace ScooterRental.Exceptions;

public class DuplicateRentedScooterException : Exception
{
    public DuplicateRentedScooterException() : base("Scooter already rented.") { }
}