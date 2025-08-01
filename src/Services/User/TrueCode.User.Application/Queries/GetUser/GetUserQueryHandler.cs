using MediatR;
using TrueCode.User.Application.DTOs;
using TrueCode.User.Domain.Interfaces;

namespace TrueCode.User.Application.Queries.GetUser;

/// <summary>
/// Обработчик запроса для получения информации о пользователе
/// </summary>
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Обрабатывает запрос получения пользователя
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о пользователе или null, если не найден</returns>
    public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            FavoriteCurrencies = user.FavoriteCurrencies,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}