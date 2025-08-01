using MediatR;
using TrueCode.Finance.Application.DTOs;

namespace TrueCode.Finance.Application.Queries.GetUserCurrencyRates;

/// <summary>
/// Запрос для получения курсов валют по пользователю
/// </summary>
/// <param name="UserId">Идентификатор пользователя</param>
public record GetUserCurrencyRatesQuery(Guid UserId) : IRequest<UserCurrencyRatesDto?>; 