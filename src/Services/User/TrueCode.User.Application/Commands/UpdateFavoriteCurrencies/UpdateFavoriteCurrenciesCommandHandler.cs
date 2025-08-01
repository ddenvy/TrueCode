using MediatR;
using TrueCode.Shared.Domain.Interfaces;
using TrueCode.User.Application.DTOs;
using TrueCode.User.Domain.Interfaces;

namespace TrueCode.User.Application.Commands.UpdateFavoriteCurrencies;

/// <summary>
/// Обработчик команды обновления избранных валют пользователя
/// </summary>
public class UpdateFavoriteCurrenciesCommandHandler : IRequestHandler<UpdateFavoriteCurrenciesCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFavoriteCurrenciesCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Обрабатывает команду обновления избранных валют
    /// </summary>
    /// <param name="request">Команда обновления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленная информация о пользователе</returns>
    /// <exception cref="InvalidOperationException">Если пользователь не найден</exception>
    public async Task<UserDto> Handle(UpdateFavoriteCurrenciesCommand request, CancellationToken cancellationToken)
    {
        // Находим пользователя
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"Пользователь с идентификатором '{request.UserId}' не найден");
        }

        // Обновляем избранные валюты
        user.SetFavoriteCurrencies(request.CurrencyCodes);

        // Сохраняем изменения
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Возвращаем обновленную информацию о пользователе
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