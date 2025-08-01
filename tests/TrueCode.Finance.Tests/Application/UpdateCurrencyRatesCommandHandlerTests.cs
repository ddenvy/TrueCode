using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TrueCode.Finance.Application.Commands.UpdateCurrencyRates;
using TrueCode.Finance.Application.DTOs;
using TrueCode.Finance.Domain.Entities;
using TrueCode.Finance.Domain.Interfaces;

namespace TrueCode.Finance.Tests.Application;

public class UpdateCurrencyRatesCommandHandlerTests
{
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly Mock<ILogger<UpdateCurrencyRatesCommandHandler>> _loggerMock;
    private readonly UpdateCurrencyRatesCommandHandler _handler;

    public UpdateCurrencyRatesCommandHandlerTests()
    {
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _loggerMock = new Mock<ILogger<UpdateCurrencyRatesCommandHandler>>();
        _handler = new UpdateCurrencyRatesCommandHandler(_currencyRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingCurrencies_WhenCurrenciesExist()
    {
        // Arrange
        var existingCurrency = Currency.Create("USD", 75.50m, "US Dollar");
        var currencyRates = new List<UpdateCurrencyRateDto>
        {
            new("USD", 76.25m),
            new("EUR", 85.30m)
        };
        var command = new UpdateCurrencyRatesCommand(currencyRates);

        _currencyRepositoryMock.Setup(x => x.GetByNameAsync("USD", It.IsAny<CancellationToken>()))
                              .ReturnsAsync(existingCurrency);
        _currencyRepositoryMock.Setup(x => x.GetByNameAsync("EUR", It.IsAny<CancellationToken>()))
                              .ReturnsAsync((Currency?)null);
        _currencyRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);
        _currencyRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<Currency>>(), It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);
        _currencyRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(2);

        _currencyRepositoryMock.Verify(x => x.GetByNameAsync("USD", It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepositoryMock.Verify(x => x.GetByNameAsync("EUR", It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<Currency>>(currencies => currencies.Count() == 1), It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Verify that existing currency rate was updated
        existingCurrency.Rate.Should().Be(76.25m);
    }

    [Fact]
    public async Task Handle_ShouldCreateNewCurrencies_WhenCurrenciesDoNotExist()
    {
        // Arrange
        var currencyRates = new List<UpdateCurrencyRateDto>
        {
            new("USD", 76.25m),
            new("EUR", 85.30m)
        };
        var command = new UpdateCurrencyRatesCommand(currencyRates);

        _currencyRepositoryMock.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync((Currency?)null);
        _currencyRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<Currency>>(), It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);
        _currencyRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(2);

        _currencyRepositoryMock.Verify(x => x.GetByNameAsync("USD", It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepositoryMock.Verify(x => x.GetByNameAsync("EUR", It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<Currency>>(currencies => currencies.Count() == 2), It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()), Times.Never);
        _currencyRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_WhenNoCurrencyRatesProvided()
    {
        // Arrange
        var command = new UpdateCurrencyRatesCommand(new List<UpdateCurrencyRateDto>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(0);

        _currencyRepositoryMock.Verify(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _currencyRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Currency>>(), It.IsAny<CancellationToken>()), Times.Never);
        _currencyRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()), Times.Never);
        _currencyRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var currencyRates = new List<UpdateCurrencyRateDto>
        {
            new("USD", 76.25m)
        };
        var command = new UpdateCurrencyRatesCommand(currencyRates);

        _currencyRepositoryMock.Setup(x => x.GetByNameAsync("USD", It.IsAny<CancellationToken>()))
                              .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("Database error");

        _currencyRepositoryMock.Verify(x => x.GetByNameAsync("USD", It.IsAny<CancellationToken>()), Times.Once);
    }
}