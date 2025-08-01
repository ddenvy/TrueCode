namespace TrueCode.Finance.Application.Interfaces;

/// <summary>
/// DTO для информации о пользователе
/// </summary>
public record UserInfoDto(
    Guid Id,
    string Name,
    List<string> FavoriteCurrencies
);

/// <summary>
/// Интерфейс для взаимодействия с пользовательским сервисом
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Получить информацию о пользователе с его избранными валютами
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о пользователе или null, если не найден</returns>
    Task<UserInfoDto?> GetUserWithFavoriteCurrenciesAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить существование пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если пользователь существует</returns>
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}