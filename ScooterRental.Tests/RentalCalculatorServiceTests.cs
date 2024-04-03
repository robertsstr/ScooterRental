using Moq;
using ScooterRental.Exceptions;
using ScooterRental.Interfaces;
using ScooterRental.Models;

namespace ScooterRental.Tests;

[TestClass]
public class RentalCalculatorServiceTests
{
    private Mock<IRentedScooterArchive> _scooterArchiveMock;
    private RentalCalculatorService _calculatorService;
    private decimal _defaultPricePerMinute = 0.1m;
    private List<RentedScooter> _rentedScooterList;

    [TestInitialize]
    public void Setup()
    {
        _rentedScooterList = new List<RentedScooter>
        {
            new RentedScooter("1", DateTime.Now.AddMinutes(-60), _defaultPricePerMinute),
            new RentedScooter("1", DateTime.Now.AddMinutes(-60), _defaultPricePerMinute)
            {
                RentEnd = DateTime.Now.AddMinutes(-10)
            }
        };

        _scooterArchiveMock = new Mock<IRentedScooterArchive>();
        _scooterArchiveMock.Setup(a => a.GetRentedScooterList()).Returns(_rentedScooterList);
        _calculatorService = new RentalCalculatorService(_scooterArchiveMock.Object);
    }

    [TestMethod]
    public void CalculateRent_ValidOverDailyLimit_ReturnRentCost()
    {
        //Arrange
        var rentStart = DateTime.Now.AddMinutes(-221);
        var rentEnd = DateTime.Now.AddMinutes(-1);
        var rentalRecord = new RentedScooter("1", rentStart, _defaultPricePerMinute);
        rentalRecord.RentEnd = rentEnd;

        //Act
        decimal actualRentCost = _calculatorService.CalculateRent(rentalRecord);

        //Assert
        Assert.AreEqual(20.0m, actualRentCost);
    }

    [TestMethod]
    public void CalculateRent_ValidForDayAndExtraMinutes_ReturnRentCost()
    {
        //Arrange
        var rentStart = DateTime.Now.AddMinutes(-1461);
        var rentEnd = DateTime.Now.AddMinutes(-1);
        var rentalRecord = new RentedScooter("1", rentStart, _defaultPricePerMinute);
        rentalRecord.RentEnd = rentEnd;

        //Act
        decimal rentCost = _calculatorService.CalculateRent(rentalRecord);

        //Assert
        Assert.AreEqual(22.0m, rentCost);
    }

    [TestMethod]
    public void CalculateRent_ValidUnderDailyLimit_ReturnRentCost()
    {
        //Arrange
        var rentStart = DateTime.Now.AddMinutes(-150);
        var rentEnd = DateTime.Now.AddMinutes(-1);
        var rentalRecord = new RentedScooter("1", rentStart, _defaultPricePerMinute);
        rentalRecord.RentEnd = rentEnd;

        //Act
        decimal rentCost = _calculatorService.CalculateRent(rentalRecord);

        //Assert
        Assert.AreEqual(14.9m, rentCost);
    }

    [TestMethod]
    public void CalculateRent_EndRentNotProvided_ThrowTimeSpanNullException()
    {
        //Arrange
        var rentStart = DateTime.Now.AddMinutes(-300);
        var rentalRecord = new RentedScooter("1", rentStart, _defaultPricePerMinute);

        //Act and Assert
        Assert.ThrowsException<TimeSpanNullException>(() => _calculatorService.CalculateRent(rentalRecord));
    }

    [TestMethod]
    public void CalculateRent_InvalidRentalDuration_ThrowInvalidRentalDurationException()
    {
        //Arrange
        var rentStart = DateTime.Now.AddMinutes(-1);
        var rentEnd = DateTime.Now.AddMinutes(-2);
        var rentalRecord = new RentedScooter("1", rentStart, _defaultPricePerMinute);
        rentalRecord.RentEnd = rentEnd;

        //Act and Assert
        Assert.ThrowsException<InvalidRentalDurationException>(() => _calculatorService.CalculateRent(rentalRecord));
    }

    [TestMethod]
    public void CalculateIncome_NoYear_IncludeNotCompletedRentalsFalse_CalculateIncome()
    {
        //Act
        decimal income = _calculatorService.CalculateIncome(null, false);

        //Assert
        Assert.AreEqual(5, income);
    }

    [TestMethod]
    public void CalculateIncome_NoYear_IncludeNotCompletedRentals_CalculateIncome()
    {
        //Act
        decimal income = _calculatorService.CalculateIncome(null , true);

        //Assert
        Assert.AreEqual(11, income);
    }

    [TestMethod]
    public void CalculateIncome_ProvidedYear_IncludeNotCompletedRentals_CalculateIncome()
    {
        //Act
        decimal income = _calculatorService.CalculateIncome(2024, true);

        //Assert
        Assert.AreEqual(11, income);
    }

    [TestMethod]
    public void CalculateIncome_NoScootersInProvidedYear_CalculateIncome()
    {
        //Act
        decimal income = _calculatorService.CalculateIncome(21, true);

        //Assert
        Assert.AreEqual(0, income);
    }
}