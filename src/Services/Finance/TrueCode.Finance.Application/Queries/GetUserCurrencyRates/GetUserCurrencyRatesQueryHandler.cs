using MediatR;
using Microsoft.Extensions.Logging;
using TrueCode.Finance.Application.DTOs;
using TrueCode.Finance.Application.Interfaces;
using TrueCode.Finance.Domain.Interfaces;

namespace TrueCode.Finance.Application.Queries.GetUserCurrencyRates;

/// <summary>
/// Обработчик запроса получения курсов валют по пользователю
/// </summary>
public class GetUserCurrencyRatesQueryHandler : IRequestHandler<GetUserCurrencyRatesQuery, UserCurrencyRatesDto?>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserService _userService;
    private readonly ILogger<GetUserCurrencyRatesQueryHandler> _logger;

    public GetUserCurrencyRatesQueryHandler(
        ICurrencyRepository currencyRepository,
        IUserService userService,
        ILogger<GetUserCurrencyRatesQueryHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Обрабатывает запрос получения курсов валют по пользователю
    /// </summary>
    /// <param name="request">Запрос с идентификатором пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Курсы валют пользователя или null, если пользователь не найден</returns>
    public async Task<UserCurrencyRatesDto?> Handle(GetUserCurrencyRatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение курсов валют для пользователя {UserId}", request.UserId);

            // Получаем информацию о пользователе и его избранных валютах
            var userInfo = await _userService.GetUserWithFavoriteCurrenciesAsync(request.UserId, cancellationToken);
            if (userInfo == null)
            {
                _logger.LogWarning("Пользователь {UserId} не найден", request.UserId);
                return null;
            }

            // Если у пользователя нет избранных валют, возвращаем пустой список
            if (!userInfo.FavoriteCurrencies.Any())
            {
                _logger.LogInformation("У пользователя {UserId} нет избранных валют", request.UserId);
                return new UserCurrencyRatesDto(
                    userInfo.Id,
                    userInfo.Name,
                    new List<CurrencyDto>(),
                    DateTime.UtcNow
                );
            }

            // Получаем курсы валют из базы данных
            var currencies = await _currencyRepository.GetByNamesAsync(userInfo.FavoriteCurrencies, cancellationToken);
            
            // Маппим в DTOs
            var currencyDtos = currencies.Select(c => new CurrencyDto(
                c.Id,
                c.Name,
                c.Rate,
                c.FullName,
                c.Nominal,
                c.CreatedAt,
                c.UpdatedAt
            )).ToList();

            _logger.LogInformation("Найдено {Count} валют для пользователя {UserId}", currencyDtos.Count, request.UserId);

            return new UserCurrencyRatesDto(
                userInfo.Id,
                userInfo.Name,
                currencyDtos,
                DateTime.UtcNow
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении курсов валют для пользователя {UserId}", request.UserId);
            throw;
        }
    }
} 