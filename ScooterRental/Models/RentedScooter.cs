namespace ScooterRental.Models;

public class RentedScooter
{
    public RentedScooter(string scooterId, DateTime rentStart, decimal pricePerMinute)
    {
        ScooterId = scooterId;
        RentStart = rentStart;
        PricePerMinute = pricePerMinute;
    }
    public string ScooterId { get; }

    public DateTime RentStart { get; set; }

    public DateTime? RentEnd { get; set; }

    public decimal PricePerMinute { get; }
}