using ScooterRental.Exceptions;
using ScooterRental.Interfaces;
using ScooterRental.Models;

namespace ScooterRental;

public class RentalCompany : IRentalCompany
{
    public string Name { get; }
    private readonly IScooterService _scooterService;
    private readonly IRentedScooterArchive _archive;
    private readonly IRentalCalculatorService _calculatorService;

    public RentalCompany(string name, IScooterService scooterService,
        IRentedScooterArchive archive, IRentalCalculatorService calculatorService)
    {
        Name = name;
        _scooterService = scooterService;
        _archive = archive;
        _calculatorService = calculatorService;
    }

    public void StartRent(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidIdException();
        }

        var scooter = _scooterService.GetScooterById(id);

        if (scooter == null)
        {
            throw new ScooterNotFoundException();
        }

        _archive.AddRentedScooter(new RentedScooter(scooter.Id, DateTime.Now, scooter.PricePerMinute));
        scooter.IsRented = true;
    }

    public decimal EndRent(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidIdException();
        }

        var scooter = _scooterService.GetScooterById(id);
        if (scooter == null)
        {
            throw new ScooterNotFoundException();
        }

        var rentalRecord = _archive.EndRental(scooter.Id, DateTime.Now);
        scooter.IsRented = false;

        return _calculatorService.CalculateRent(rentalRecord);
    }

    public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
    {
        return _calculatorService.CalculateIncome(year, includeNotCompletedRentals);
    }
}