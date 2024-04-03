using ScooterRental.Exceptions;
using ScooterRental.Interfaces;
using ScooterRental.Models;

namespace ScooterRental;

public class RentedScooterArchive : IRentedScooterArchive
{
    private readonly List<RentedScooter> _rentedScooters;

    public RentedScooterArchive(List<RentedScooter> scooters)
    {
        _rentedScooters = scooters;
    }

    public void AddRentedScooter(RentedScooter scooter)
    {
        if (_rentedScooters.Any(rentedScooter
                => rentedScooter.ScooterId == scooter.ScooterId && rentedScooter.RentStart == scooter.RentStart))
        {
            throw new DuplicateRentedScooterException();
        }

        _rentedScooters.Add(scooter);
    }

    public RentedScooter EndRental(string scooterId, DateTime rentEnd)
    {
        var rentedScooter = _rentedScooters.Find(scooter => scooter
            .ScooterId == scooterId);
        if (rentedScooter == null)
        {
            throw new ScooterNotFoundException();
        }

        if (rentedScooter.RentEnd != null)
        {
            throw new ScooterRentEndedException();
        }

        if (rentEnd < rentedScooter.RentStart)
        {
            throw new InvalidDateTimeException();
        }

        rentedScooter.RentEnd = rentEnd;
        return rentedScooter;
    }

    public List<RentedScooter> GetRentedScooterList()
    {
        return _rentedScooters;
    }
}