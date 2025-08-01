using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrueCode.User.Application.Commands.LoginUser;
using TrueCode.User.Application.Commands.RegisterUser;
using TrueCode.User.Application.Commands.UpdateFavoriteCurrencies;
using TrueCode.User.Application.DTOs;
using TrueCode.User.Application.Queries.GetUser;

namespace TrueCode.User.API.Controllers;

/// <summary>
/// Контроллер для работы с пользователями
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="request">Данные для регистрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Токен аутентификации и информация о пользователе</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RegisterUserCommand(request.Name, request.Password);
            var result = await _mediator.Send(command, cancellationToken);
            
            _logger.LogInformation("Пользователь {UserName} успешно зарегистрирован", request.Name);
            return CreatedAtAction(nameof(GetProfile), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Ошибка регистрации пользователя {UserName}: {Error}", request.Name, ex.Message);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при регистрации пользователя {UserName}", request.Name);
            return BadRequest("Произошла ошибка при регистрации пользователя");
        }
    }

    /// <summary>
    /// Вход пользователя в систему
    /// </summary>
    /// <param name="request">Данные для входа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Токен аутентификации и информация о пользователе</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new LoginUserCommand(request.Name, request.Password);
            var result = await _mediator.Send(command, cancellationToken);
            
            _logger.LogInformation("Пользователь {UserName} успешно вошел в систему", request.Name);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Неудачная попытка входа для пользователя {UserName}: {Error}", request.Name, ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при входе пользователя {UserName}", request.Name);
            return BadRequest("Произошла ошибка при входе в систему");
        }
    }

    /// <summary>
    /// Получение профиля текущего пользователя
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о пользователе</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var query = new GetUserQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);
            
            if (result == null)
            {
                _logger.LogWarning("Пользователь с ID {UserId} не найден", userId);
                return NotFound("Пользователь не найден");
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении профиля пользователя");
            return BadRequest("Произошла ошибка при получении профиля");
        }
    }

    /// <summary>
    /// Обновление избранных валют пользователя
    /// </summary>
    /// <param name="request">Список валют</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленная информация о пользователе</returns>
    [HttpPut("favorite-currencies")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFavoriteCurrencies([FromBody] UpdateFavoriteCurrenciesDto request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var command = new UpdateFavoriteCurrenciesCommand(userId, request.CurrencyCodes);
            var result = await _mediator.Send(command, cancellationToken);
            
            _logger.LogInformation("Пользователь {UserId} обновил избранные валюты", userId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Ошибка обновления избранных валют: {Error}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при обновлении избранных валют");
            return BadRequest("Произошла ошибка при обновлении избранных валют");
        }
    }

    /// <summary>
    /// Выход пользователя из системы
    /// </summary>
    /// <returns>Результат операции</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        // В случае с JWT токенами, логаут обычно обрабатывается на клиентской стороне
        // путем удаления токена из хранилища клиента.
        // Здесь мы просто возвращаем успешный ответ.
        _logger.LogInformation("Пользователь {UserId} вышел из системы", GetCurrentUserId());
        return Ok(new { message = "Успешный выход из системы" });
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