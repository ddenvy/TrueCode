namespace TrueCode.Shared.Domain.Interfaces;

/// <summary>
/// Интерфейс единицы работы для управления транзакциями
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Сохранить все изменения в базе данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество затронутых записей</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Начать транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Подтвердить транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Откатить транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}