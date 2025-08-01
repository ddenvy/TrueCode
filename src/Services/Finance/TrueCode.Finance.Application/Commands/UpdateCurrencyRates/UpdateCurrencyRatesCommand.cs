using MediatR;
using TrueCode.Finance.Application.DTOs;

namespace TrueCode.Finance.Application.Commands.UpdateCurrencyRates;

/// <summary>
/// Команда для массового обновления курсов валют
/// </summary>
/// <param name="Currencies">Список валют для обновления</param>
public record UpdateCurrencyRatesCommand(List<UpdateCurrencyRateDto> Currencies) : IRequest<int>; 