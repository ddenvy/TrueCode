using FluentAssertions;
using TrueCode.Finance.Domain.Entities;

namespace TrueCode.Finance.Tests.Domain;

public class CurrencyEntityTests
{
    [Fact]
    public void Create_ShouldCreateCurrencyWithValidData()
    {
        // Arrange
        var name = "USD";
        var rate = 75.50m;
        var fullName = "US Dollar";
        var nominal = 1;

        // Act
        var currency = Currency.Create(name, rate, fullName, nominal);

        // Assert
        currency.Should().NotBeNull();
        currency.Name.Should().Be(name);
        currency.Rate.Should().Be(rate);
        currency.FullName.Should().Be(fullName);
        currency.Nominal.Should().Be(nominal);
        currency.Id.Should().NotBeEmpty();
        currency.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        currency.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldUseDefaultNominal_WhenNotProvided()
    {
        // Arrange
        var name = "EUR";
        var rate = 85.30m;
        var fullName = "Euro";

        // Act
        var currency = Currency.Create(name, rate, fullName);

        // Assert
        currency.Nominal.Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_ShouldThrowArgumentException_WhenNameIsInvalid(string invalidName)
    {
        // Arrange
        var rate = 75.50m;
        var fullName = "US Dollar";

        // Act & Assert
        var act = () => Currency.Create(invalidName, rate, fullName);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*name*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Create_ShouldThrowArgumentException_WhenRateIsInvalid(decimal invalidRate)
    {
        // Arrange
        var name = "USD";
        var fullName = "US Dollar";

        // Act & Assert
        var act = () => Currency.Create(name, invalidRate, fullName);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*rate*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_ShouldThrowArgumentException_WhenFullNameIsInvalid(string invalidFullName)
    {
        // Arrange
        var name = "USD";
        var rate = 75.50m;

        // Act & Assert
        var act = () => Currency.Create(name, rate, invalidFullName);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*fullName*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Create_ShouldThrowArgumentException_WhenNominalIsInvalid(int invalidNominal)
    {
        // Arrange
        var name = "USD";
        var rate = 75.50m;
        var fullName = "US Dollar";

        // Act & Assert
        var act = () => Currency.Create(name, rate, fullName, invalidNominal);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*nominal*");
    }

    [Fact]
    public void UpdateRate_ShouldUpdateRateAndTimestamp()
    {
        // Arrange
        var currency = Currency.Create("USD", 75.50m, "US Dollar");
        var newRate = 76.25m;
        var originalUpdatedAt = currency.UpdatedAt;

        // Act
        currency.UpdateRate(newRate);

        // Assert
        currency.Rate.Should().Be(newRate);
        currency.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        currency.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void UpdateRate_ShouldThrowArgumentException_WhenRateIsInvalid(decimal invalidRate)
    {
        // Arrange
        var currency = Currency.Create("USD", 75.50m, "US Dollar");

        // Act & Assert
        var act = () => currency.UpdateRate(invalidRate);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*rate*");
    }

    [Fact]
    public void UpdateInfo_ShouldUpdateAllFieldsAndTimestamp()
    {
        // Arrange
        var currency = Currency.Create("USD", 75.50m, "US Dollar", 1);
        var newRate = 76.25m;
        var newFullName = "United States Dollar";
        var newNominal = 10;
        var originalUpdatedAt = currency.UpdatedAt;

        // Act
        currency.UpdateInfo(newRate, newFullName, newNominal);

        // Assert
        currency.Rate.Should().Be(newRate);
        currency.FullName.Should().Be(newFullName);
        currency.Nominal.Should().Be(newNominal);
        currency.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        currency.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
