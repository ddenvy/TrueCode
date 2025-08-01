using TrueCode.Shared.Domain.Interfaces;
using TrueCode.User.Domain.Entities;

namespace TrueCode.User.Domain.Interfaces;

/// <summary>
/// Интерфейс репозитория для работы с пользователями
/// </summary>
public interface IUserRepository : IRepository<Entities.User>
{
    /// <summary>
    /// Получить пользователя по имени
    /// </summary>
    /// <param name="name">Имя пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Пользователь или null, если не найден</returns>
    Task<Entities.User?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить существование пользователя с указанным именем
    /// </summary>
    /// <param name="name">Имя пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если пользователь существует</returns>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить существование пользователя с указанным именем, исключая указанного пользователя
    /// </summary>
    /// <param name="name">Имя пользователя</param>
    /// <param name="excludeUserId">Идентификатор пользователя для исключения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если пользователь существует</returns>
    Task<bool> ExistsByNameAsync(string name, Guid excludeUserId, CancellationToken cancellationToken = default);
}