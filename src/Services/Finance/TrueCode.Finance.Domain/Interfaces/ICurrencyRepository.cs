using TrueCode.Finance.Domain.Entities;

namespace TrueCode.Finance.Domain.Interfaces;

/// <summary>
/// Интерфейс репозитория для работы с валютами
/// </summary>
public interface ICurrencyRepository
{
    /// <summary>
    /// Получить валюту по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Валюта или null, если не найдена</returns>
    Task<Currency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить валюту по коду (названию)
    /// </summary>
    /// <param name="name">Код валюты (USD, EUR, etc.)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Валюта или null, если не найдена</returns>
    Task<Currency?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить все валюты
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список всех валют</returns>
    Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить валюты по списку кодов
    /// </summary>
    /// <param name="names">Список кодов валют</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список валют</returns>
    Task<List<Currency>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Добавить новую валюту
    /// </summary>
    /// <param name="currency">Валюта для добавления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task AddAsync(Currency currency, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Добавить несколько валют
    /// </summary>
    /// <param name="currencies">Валюты для добавления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task AddRangeAsync(IEnumerable<Currency> currencies, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновить существующую валюту
    /// </summary>
    /// <param name="currency">Валюта для обновления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить валюту
    /// </summary>
    /// <param name="currency">Валюта для удаления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task DeleteAsync(Currency currency, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Сохранить изменения в базе данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество затронутых записей</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить существование валюты по коду
    /// </summary>
    /// <param name="name">Код валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если валюта существует</returns>
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
}