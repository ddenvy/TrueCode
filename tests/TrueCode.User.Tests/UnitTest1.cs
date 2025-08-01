using FluentAssertions;
using TrueCode.User.Domain.Entities;

namespace TrueCode.User.Tests.Domain;

public class UserEntityTests
{
    [Fact]
    public void Create_ShouldCreateUserWithValidData()
    {
        // Arrange
        var name = "testuser";
        var hashedPassword = "hashedpassword123";

        // Act
        var user = TrueCode.User.Domain.Entities.User.Create(name, hashedPassword);

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be(name);
        user.Password.Should().Be(hashedPassword);
        user.FavoriteCurrencies.Should().BeEmpty();
        user.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_ShouldThrowArgumentException_WhenNameIsInvalid(string invalidName)
    {
        // Arrange
        var hashedPassword = "hashedpassword123";

        // Act & Assert
        var act = () => TrueCode.User.Domain.Entities.User.Create(invalidName, hashedPassword);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*name*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_ShouldThrowArgumentException_WhenPasswordIsInvalid(string invalidPassword)
    {
        // Arrange
        var name = "testuser";

        // Act & Assert
        var act = () => TrueCode.User.Domain.Entities.User.Create(name, invalidPassword);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*password*");
    }

    [Fact]
    public void SetFavoriteCurrencies_ShouldUpdateFavoriteCurrencies()
    {
        // Arrange
        var user = TrueCode.User.Domain.Entities.User.Create("testuser", "hashedpassword123");
        var currencies = new[] { "USD", "EUR", "GBP" };

        // Act
        user.SetFavoriteCurrencies(currencies);

        // Assert
        user.FavoriteCurrencies.Should().BeEquivalentTo(currencies);
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetFavoriteCurrencies_ShouldHandleEmptyList()
    {
        // Arrange
        var user = TrueCode.User.Domain.Entities.User.Create("testuser", "hashedpassword123");
        user.SetFavoriteCurrencies(new[] { "USD", "EUR" });
        
        // Act
        user.SetFavoriteCurrencies(new string[0]);

        // Assert
        user.FavoriteCurrencies.Should().BeEmpty();
    }

    [Fact]
    public void SetFavoriteCurrencies_ShouldHandleNullInput()
    {
        // Arrange
        var user = TrueCode.User.Domain.Entities.User.Create("testuser", "hashedpassword123");

        // Act & Assert
        var act = () => user.SetFavoriteCurrencies(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SetFavoriteCurrencies_ShouldRemoveDuplicates()
    {
        // Arrange
        var user = TrueCode.User.Domain.Entities.User.Create("testuser", "hashedpassword123");
        var currenciesWithDuplicates = new[] { "USD", "EUR", "USD", "GBP", "EUR" };

        // Act
        user.SetFavoriteCurrencies(currenciesWithDuplicates);

        // Assert
        user.FavoriteCurrencies.Should().BeEquivalentTo(new[] { "USD", "EUR", "GBP" });
        user.FavoriteCurrencies.Should().HaveCount(3);
    }
}
