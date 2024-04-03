using ScooterRental.Exceptions;
using ScooterRental.Interfaces;
using ScooterRental.Models;

namespace ScooterRental;

public class RentalCalculatorService : IRentalCalculatorService
{
    private readonly decimal _maxDailyPrice = 20.0m;
    private readonly IRentedScooterArchive _scooterArchive;
    private readonly List<RentedScooter> _rentedScooterList;

    public RentalCalculatorService(IRentedScooterArchive scooterArchive)
    {
        _scooterArchive = scooterArchive;
        _rentedScooterList = scooterArchive.GetRentedScooterList();
    }

    public decimal CalculateRent(RentedScooter rentalRecord)
    {
        TimeSpan? timeSpan = rentalRecord.RentEnd - rentalRecord.RentStart;
        if (timeSpan == null)
        {
            throw new TimeSpanNullException();
        }

        TimeSpan rentalDuration = timeSpan.Value;
        if (rentalDuration.TotalMinutes <= 0)
        {
            throw new InvalidRentalDurationException();
        }

        int totalDays = rentalDuration.Days;
        int remainingHours = rentalDuration.Hours;
        int remainingMinutes = rentalDuration.Minutes;

        decimal totalRentCost = 0;

        totalRentCost += totalDays * _maxDailyPrice;

        int totalRemainingMinutes = remainingHours * 60 + remainingMinutes;
        decimal remainingRentCost = totalRemainingMinutes * rentalRecord.PricePerMinute;

        if (remainingRentCost > _maxDailyPrice)
        {
            return totalRentCost += _maxDailyPrice;
        }

        return totalRentCost += remainingRentCost;
    }

    public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
    {
        decimal income = 0.0m;

        foreach (var scooter in _rentedScooterList)
        {
            if (includeNotCompletedRentals && scooter.RentEnd == null)
            {
                scooter.RentEnd = DateTime.Now;
            }

            if (scooter.RentEnd != null && (!year.HasValue || scooter.RentStart.Year == year.Value))
            {
                income += CalculateRent(scooter);
            }
        }

        return income;
    }
}