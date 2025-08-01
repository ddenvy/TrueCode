using TrueCode.Shared.Domain.Entities;

namespace TrueCode.Shared.Domain.Interfaces;

/// <summary>
/// Базовый интерфейс репозитория для работы с сущностями
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Получить сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Сущность или null, если не найдена</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить все сущности
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция сущностей</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Добавить новую сущность
    /// </summary>
    /// <param name="entity">Сущность для добавления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Добавленная сущность</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновить существующую сущность
    /// </summary>
    /// <param name="entity">Сущность для обновления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленная сущность</returns>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="entity">Сущность для удаления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
}