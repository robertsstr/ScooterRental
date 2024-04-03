using Moq;
using ScooterRental.Exceptions;
using ScooterRental.Interfaces;
using ScooterRental.Models;

namespace ScooterRental.Tests;

[TestClass]
public class RentalCompanyTests
{
    private Mock<IScooterService> _scooterServiceMock;
    private Mock<IRentedScooterArchive> _rentedScooterArchiveMock;
    private Mock<IRentalCalculatorService> _calculatorServiceMock;
    private RentalCompany _company;
    private readonly string _defaultScooterId = "1";
    private readonly decimal _defaultScooterPrice = 0.1m;

    [TestInitialize]
    public void Setup()
    {
        _scooterServiceMock = new Mock<IScooterService>();
        _rentedScooterArchiveMock = new Mock<IRentedScooterArchive>();
        _calculatorServiceMock = new Mock<IRentalCalculatorService>();
        _company = new RentalCompany("Test", _scooterServiceMock.Object, _rentedScooterArchiveMock.Object,
            _calculatorServiceMock.Object);
    }

    [TestMethod]
    public void StartRent_RentExistingScooter_ScooterIsRentedTrue()
    {
        //Arrange
        var scooter = new Scooter(_defaultScooterId, _defaultScooterPrice);
        _scooterServiceMock.Setup(scooters => scooters.GetScooterById(_defaultScooterId))
            .Returns(scooter);

        //Act
        _company.StartRent(_defaultScooterId);

        //Assert
        Assert.IsTrue(scooter.IsRented);
    }

    [TestMethod]
    public void StartRent_InvalidScooterId_InvalidIdException()
    {
        //Act and Assert
        Assert.ThrowsException<InvalidIdException>(() => _company.StartRent(""));
    }

    [TestMethod]
    public void StartRent_NonExistingScooter_ScooterNotFoundException()
    {
        // Arrange
        _scooterServiceMock.Setup(scooters => scooters.GetScooterById(_defaultScooterId))
            .Returns((Scooter)null);

        // Act & Assert
        Assert.ThrowsException<ScooterNotFoundException>(() => _company.StartRent(_defaultScooterId));
    }

    [TestMethod]
    public void EndRent_StopRentingExistingScooter_ScooterIsRentedFalse()
    {
        //Arrange
        var scooter = new Scooter(_defaultScooterId, _defaultScooterPrice){ IsRented = true };
        var now = DateTime.Now;
        var rentalRecord = new RentedScooter(scooter.Id, now.AddMinutes(-20), scooter.PricePerMinute){RentEnd = now};
        _scooterServiceMock.Setup(scooters => scooters.GetScooterById(_defaultScooterId)).Returns(scooter);
        _rentedScooterArchiveMock.Setup(archive => archive.EndRental(scooter.Id, It.IsAny<DateTime>()))
            .Returns(rentalRecord);
        _calculatorServiceMock.Setup(calculator => calculator.CalculateRent(rentalRecord)).Returns(2);

        //Act
        var rentResult = _company.EndRent(_defaultScooterId);

        //Assert
        Assert.IsFalse(scooter.IsRented);
        Assert.AreEqual(2, rentResult);
    }

    [TestMethod]
    public void EndRent_NonRentedScooter_ReturnsZeroRent()
    {
        // Arrange
        var nonRentedScooter = new Scooter(_defaultScooterId, _defaultScooterPrice) { IsRented = false };
        _scooterServiceMock.Setup(s => s.GetScooterById(_defaultScooterId)).Returns(nonRentedScooter);

        // Act
        var rentResult = _company.EndRent(_defaultScooterId);

        // Assert
        Assert.AreEqual(0, rentResult);
    }

    [TestMethod]
    public void EndRent_CalculateRentBasedOnDuration_ReturnsCorrectRent()
    {
        // Arrange
        var rentedScooter = new Scooter(_defaultScooterId, _defaultScooterPrice) { IsRented = true };
        var now = DateTime.Now;
        var rentalRecord = new RentedScooter(rentedScooter.Id, now.AddMinutes(-30), rentedScooter.PricePerMinute) { RentEnd = now };
        _scooterServiceMock.Setup(s 
            => s.GetScooterById(_defaultScooterId)).Returns(rentedScooter);
        _rentedScooterArchiveMock.Setup(a 
            => a.EndRental(rentedScooter.Id, It.IsAny<DateTime>())).Returns(rentalRecord);
        _calculatorServiceMock.Setup(c 
            => c.CalculateRent(rentalRecord)).Returns(3.5m);

        // Act
        var rent = _company.EndRent(_defaultScooterId);

        // Assert
        Assert.AreEqual(3.5m, rent, "Rent should be calculated based on the duration of the rental.");
    }

    [TestMethod]
    public void EndRent_InvalidScooterId_InvalidIdException()
    {
        // Act & Assert
        Assert.ThrowsException<InvalidIdException>(() => _company.EndRent(""));
    }

    [TestMethod]
    public void EndRent_NonExistingScooter_ScooterNotFoundException()
    {
        // Arrange
        _scooterServiceMock.Setup(s => s.GetScooterById(_defaultScooterId)).Returns((Scooter)null);

        // Act & Assert
        Assert.ThrowsException<ScooterNotFoundException>(() => _company.EndRent(_defaultScooterId));
    }
}