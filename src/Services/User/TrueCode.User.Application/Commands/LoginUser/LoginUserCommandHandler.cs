using MediatR;
using TrueCode.Shared.Infrastructure.Services;
using TrueCode.User.Application.DTOs;
using TrueCode.User.Domain.Interfaces;

namespace TrueCode.User.Application.Commands.LoginUser;

/// <summary>
/// Обработчик команды входа пользователя
/// </summary>
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtService _jwtService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IJwtService jwtService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }

    /// <summary>
    /// Обрабатывает команду входа пользователя
    /// </summary>
    /// <param name="request">Команда входа</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ с токеном и информацией о пользователе</returns>
    /// <exception cref="UnauthorizedAccessException">Если учетные данные неверны</exception>
    public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // Находим пользователя по имени
        var user = await _userRepository.GetByNameAsync(request.Name, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");
        }

        // Проверяем пароль
        if (!_passwordHashService.VerifyPassword(request.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");
        }

        // Генерируем JWT токен
        var token = _jwtService.GenerateToken(user.Id, user.Name);

        // Возвращаем ответ
        return new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                FavoriteCurrencies = user.FavoriteCurrencies,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };
    }
}