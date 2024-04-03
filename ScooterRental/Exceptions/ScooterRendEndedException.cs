namespace ScooterRental.Exceptions;

public class ScooterRentEndedException : Exception
{
    public ScooterRentEndedException() : base("Scooters rent is already over.") { }
}