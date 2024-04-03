using ScooterRental.Exceptions;
using ScooterRental.Models;

namespace ScooterRental.Tests;

[TestClass]
public class ScooterServiceTests
{
    private ScooterService _scooterService;
    private List<Scooter> _scooters;
    private string _defaultId = "1";
    private decimal _defaultPrice = 0.1m;

    [TestInitialize]
    public void Setup()
    {
        _scooters = new List<Scooter>();
        _scooterService = new ScooterService(_scooters);
    }
        
    [TestMethod]
    public void AddScooter_ValidDataProvided_ScooterAdded()
    {
        //Act
        _scooterService.AddScooter("1", 0.1m);

        //Assert
        Assert.AreEqual(1, _scooters.Count);
    }

    [TestMethod]
    public void AddScooter_InvalidScooterId_ThrowInvalidIdException()
    {
        //Act and Assert
        Assert.ThrowsException<InvalidIdException>(() => _scooterService.AddScooter("", _defaultPrice));
    }

    [TestMethod]
    public void AddScooter_DuplicateId_ThrowDuplicateException()
    {
        //Arrange
        _scooterService.AddScooter(_defaultId, _defaultPrice);

        //Act and Assert
        Assert.ThrowsException<DuplicateScooterException>(() => _scooterService.AddScooter(_defaultId, _defaultPrice));
    }

    [DataTestMethod]
    [DataRow("0")]
    [DataRow("-0.1")]
    [DataRow("-12345")]

    public void AddScooter_InvalidScooterPrice_ThrowInvalidPriceException(string pricePerMinute)
    {
        //Arrange
        decimal price = decimal.Parse(pricePerMinute);

        //Act and Assert
        Assert.ThrowsException<InvalidPriceException>(() => _scooterService.AddScooter(_defaultId, price));
    }

    [TestMethod]
    public void RemoveScooter_ValidScooterId_ScooterRemoved()
    {
        //Arrange
        _scooterService.AddScooter(_defaultId, _defaultPrice);

        //Act
        _scooterService.RemoveScooter(_defaultId);

        //Assert
        Assert.AreEqual(0, _scooters.Count);
    }

    [TestMethod]
    public void RemoveScooter_NonExistingScooterId_ThrowScooterNotFoundException()
    {
        //Arrange
        _scooterService.AddScooter("2", _defaultPrice);

        //Act and Assert
        Assert.ThrowsException<ScooterNotFoundException>(() => _scooterService.RemoveScooter(_defaultId));
    }

    [TestMethod]
    public void GetScooters_ReturnScooterList()
    {
        // Arrange
        _scooterService.AddScooter(_defaultId, _defaultPrice);
        _scooterService.AddScooter("2", 0.34m);
        _scooterService.AddScooter("3", 2);

        // Act
        var scooters = _scooterService.GetScooters();

        // Assert
        Assert.IsNotNull(scooters);
        Assert.AreEqual(3, scooters.Count);
        Assert.IsTrue(scooters.Any(s => s.Id == _defaultId && s.PricePerMinute == _defaultPrice));
        Assert.IsTrue(scooters.Any(s => s.Id == "2" && s.PricePerMinute == 0.34m));
        Assert.IsTrue(scooters.Any(s => s.Id == "3" && s.PricePerMinute == 2));
    }

    [TestMethod]
    public void GetScooterById_ExistingScooterId_ReturnScooter()
    {
        // Arrange
        _scooterService.AddScooter(_defaultId, _defaultPrice);

        // Act
        var foundScooter = _scooterService.GetScooterById(_defaultId);

        // Assert
        Assert.AreEqual(_defaultId, foundScooter.Id);
    }

    [TestMethod]
    public void GetScooterById_NonExistingScooter_ThrowsScooterNotFoundException()
    {
        //Arrange
        _scooterService.AddScooter("2", _defaultPrice);

        //Act and Assert
        Assert.ThrowsException<ScooterNotFoundException>(() => _scooterService.GetScooterById(_defaultId));
    }
}