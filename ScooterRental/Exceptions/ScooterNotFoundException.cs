namespace ScooterRental.Exceptions;

public class ScooterNotFoundException : Exception
{
    public ScooterNotFoundException() : base("Scooter ID not found.") { }
}