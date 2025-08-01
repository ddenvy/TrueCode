using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrueCode.Finance.Application.DTOs;
using TrueCode.Finance.Application.Queries.GetUserCurrencyRates;

namespace TrueCode.Finance.API.Controllers;

/// <summary>
/// Контроллер для работы с финансовыми данными
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FinanceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FinanceController> _logger;

    public FinanceController(IMediator mediator, ILogger<FinanceController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получить курсы валют для текущего пользователя
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Курсы избранных валют пользователя</returns>
    [HttpGet("user-rates")]
    [Authorize]
    [ProducesResponseType(typeof(UserCurrencyRatesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserCurrencyRates(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Запрос курсов валют для пользователя {UserId}", userId);

            var query = new GetUserCurrencyRatesQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
            {
                _logger.LogWarning("Пользователь {UserId} не найден", userId);
                return NotFound("Пользователь не найден");
            }

            _logger.LogInformation("Возвращено {Count} валют для пользователя {UserId}", 
                result.FavoriteCurrencies.Count, userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении курсов валют для пользователя");
            return BadRequest("Произошла ошибка при получении курсов валют");
        }
    }

    /// <summary>
    /// Получить курсы валют для указанного пользователя (для администраторов)
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Курсы избранных валют пользователя</returns>
    [HttpGet("user-rates/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(UserCurrencyRatesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserCurrencyRates(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Запрос курсов валют для пользователя {UserId}", userId);

            var query = new GetUserCurrencyRatesQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
            {
                _logger.LogWarning("Пользователь {UserId} не найден", userId);
                return NotFound("Пользователь не найден");
            }

            _logger.LogInformation("Возвращено {Count} валют для пользователя {UserId}", 
                result.FavoriteCurrencies.Count, userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении курсов валют для пользователя {UserId}", userId);
            return BadRequest("Произошла ошибка при получении курсов валют");
        }
    }

    /// <summary>
    /// Получить все доступные валюты
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список всех валют</returns>
    [HttpGet("currencies")]
    [ProducesResponseType(typeof(List<CurrencyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCurrencies(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Запрос всех доступных валют");

            // TODO: Реализовать GetAllCurrenciesQuery
            // var query = new GetAllCurrenciesQuery();
            // var result = await _mediator.Send(query, cancellationToken);

            // Временная заглушка
            var result = new List<CurrencyDto>();
            
            _logger.LogInformation("Возвращено {Count} валют", result.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка валют");
            return BadRequest("Произошла ошибка при получении списка валют");
        }
    }

    /// <summary>
    /// Получает идентификатор текущего пользователя из токена
    /// </summary>
    /// <returns>Идентификатор пользователя</returns>
    /// <exception cref="UnauthorizedAccessException">Если пользователь не аутентифицирован</exception>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Пользователь не аутентифицирован");
        }
        return userId;
    }
} 