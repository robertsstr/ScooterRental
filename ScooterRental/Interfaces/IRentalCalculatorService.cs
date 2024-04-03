using ScooterRental.Models;

namespace ScooterRental.Interfaces;

public interface IRentalCalculatorService
{
    decimal CalculateRent(RentedScooter rentalRecord);
    decimal CalculateIncome(int? year, bool includeNotCompletedRentals);
}