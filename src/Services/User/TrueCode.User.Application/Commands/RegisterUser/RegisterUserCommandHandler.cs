using MediatR;
using TrueCode.Shared.Domain.Interfaces;
using TrueCode.Shared.Infrastructure.Services;
using TrueCode.User.Application.DTOs;
using TrueCode.User.Domain.Interfaces;

namespace TrueCode.User.Application.Commands.RegisterUser;

/// <summary>
/// Обработчик команды регистрации пользователя
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtService _jwtService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHashService passwordHashService,
        IJwtService jwtService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }

    /// <summary>
    /// Обрабатывает команду регистрации пользователя
    /// </summary>
    /// <param name="request">Команда регистрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ с токеном и информацией о пользователе</returns>
    /// <exception cref="InvalidOperationException">Если пользователь с таким именем уже существует</exception>
    public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Проверяем, что пользователь с таким именем не существует
        var existingUser = await _userRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Пользователь с именем '{request.Name}' уже существует");
        }

        // Хешируем пароль
        var hashedPassword = _passwordHashService.HashPassword(request.Password);

        // Создаем нового пользователя
        var user = Domain.Entities.User.Create(request.Name, hashedPassword);

        // Сохраняем пользователя в базе данных
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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