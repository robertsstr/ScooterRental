using ScooterRental.Models;

namespace ScooterRental.Interfaces;

public interface IRentedScooterArchive
{
    void AddRentedScooter(RentedScooter scooter);

    RentedScooter EndRental(string scooterId, DateTime rentEnd);

    List<RentedScooter> GetRentedScooterList();
}