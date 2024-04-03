using ScooterRental.Exceptions;
using ScooterRental.Models;

namespace ScooterRental.Tests;

[TestClass]
public class RentedScooterArchiveTests
{
    private List<RentedScooter> _rentedScooters;
    private RentedScooterArchive _archive;
    private string _defaultScooterId = "1";
    private DateTime _defaultTime = DateTime.Now;
    private decimal _defaultPrice = 0.1m;

    [TestInitialize]
    public void Setup()
    {
        _rentedScooters = new List<RentedScooter>()
        {
            new RentedScooter(_defaultScooterId, _defaultTime, _defaultPrice)
        };

        _archive = new RentedScooterArchive(_rentedScooters);
    }

    [TestMethod]
    public void AddRentedScooter_ValidRentedScooter_AddsScooterToArchive()
    {
        // Act
        _archive.AddRentedScooter(new RentedScooter("2", _defaultTime, _defaultPrice));

        // Assert
        Assert.AreEqual(2, _rentedScooters.Count);
    }

    [TestMethod]
    public void AddRentalScooter_DuplicateRentedScooter_ThrowsDuplicateRentedScooterException()
    {
        //Act and Assert
        Assert.ThrowsException<DuplicateRentedScooterException>(() =>
            _archive.AddRentedScooter(new RentedScooter(_defaultScooterId, _defaultTime, _defaultPrice)));
    }

    [TestMethod]
    public void EndRental_ValidScooterRental_ReturnRentedScooter()
    {
        //Arrange
        var rentEnd = _defaultTime.AddMinutes(20);

        //Act
        var rentedScooter = _archive.EndRental(_defaultScooterId, rentEnd);

        //Assert
        Assert.IsNotNull(rentedScooter);
        Assert.AreEqual(_defaultScooterId, rentedScooter.ScooterId);
        Assert.AreEqual(rentEnd, rentedScooter.RentEnd);
    }

    [TestMethod]
    public void EndRental_ScooterNotFound_ThrowsScooterNotFoundException()
    {
        // Arrange
        string nonExistentScooterId = "2";
        var rentEnd = _defaultTime.AddMinutes(20);

        // Act & Assert
        Assert.ThrowsException<ScooterNotFoundException>(() =>
            _archive.EndRental(nonExistentScooterId, rentEnd));
    }

    [TestMethod]
    public void EndRental_ScooterAlreadyEnded_ThrowsScooterRentEndedException()
    {
        // Arrange
        var rentEnd = _defaultTime.AddMinutes(20);
        _archive.AddRentedScooter(new RentedScooter("2", _defaultTime, _defaultPrice){RentEnd = rentEnd});
            
        // Act & Assert
        Assert.ThrowsException<ScooterRentEndedException>(() =>
            _archive.EndRental("2", rentEnd));
    }

    [TestMethod]
    public void EndRental_InvalidRentEnd_ThrowsInvalidDataException()
    {
        // Arrange
        DateTime rentEnd = _defaultTime.AddMinutes(-2);

        // Act & Assert
        Assert.ThrowsException<InvalidDateTimeException>(() =>
            _archive.EndRental(_defaultScooterId, rentEnd));
    }
}