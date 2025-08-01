using TrueCode.Shared.Domain.Entities;

namespace TrueCode.User.Domain.Entities;

/// <summary>
/// Доменная модель пользователя
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Хешированный пароль пользователя
    /// </summary>
    public string Password { get; private set; } = string.Empty;
    
    /// <summary>
    /// Список избранных валют пользователя
    /// </summary>
    public List<string> FavoriteCurrencies { get; private set; } = new();
    
    /// <summary>
    /// Приватный конструктор для Entity Framework
    /// </summary>
    private User() { }
    
    /// <summary>
    /// Создает нового пользователя
    /// </summary>
    /// <param name="name">Имя пользователя</param>
    /// <param name="hashedPassword">Хешированный пароль</param>
    /// <returns>Новый экземпляр пользователя</returns>
    public static User Create(string name, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя пользователя не может быть пустым", nameof(name));
        
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Пароль не может быть пустым", nameof(hashedPassword));
        
        return new User
        {
            Name = name.Trim(),
            Password = hashedPassword
        };
    }
    
    /// <summary>
    /// Обновляет имя пользователя
    /// </summary>
    /// <param name="name">Новое имя пользователя</param>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя пользователя не может быть пустым", nameof(name));
        
        Name = name.Trim();
        UpdateTimestamp();
    }
    
    /// <summary>
    /// Обновляет пароль пользователя
    /// </summary>
    /// <param name="hashedPassword">Новый хешированный пароль</param>
    public void UpdatePassword(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Пароль не может быть пустым", nameof(hashedPassword));
        
        Password = hashedPassword;
        UpdateTimestamp();
    }
    
    /// <summary>
    /// Добавляет валюту в избранные
    /// </summary>
    /// <param name="currencyCode">Код валюты</param>
    public void AddFavoriteCurrency(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Код валюты не может быть пустым", nameof(currencyCode));
        
        var normalizedCode = currencyCode.Trim().ToUpperInvariant();
        
        if (!FavoriteCurrencies.Contains(normalizedCode))
        {
            FavoriteCurrencies.Add(normalizedCode);
            UpdateTimestamp();
        }
    }
    
    /// <summary>
    /// Удаляет валюту из избранных
    /// </summary>
    /// <param name="currencyCode">Код валюты</param>
    public void RemoveFavoriteCurrency(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return;
        
        var normalizedCode = currencyCode.Trim().ToUpperInvariant();
        
        if (FavoriteCurrencies.Remove(normalizedCode))
        {
            UpdateTimestamp();
        }
    }
    
    /// <summary>
    /// Устанавливает список избранных валют
    /// </summary>
    /// <param name="currencyCodes">Коды валют</param>
    public void SetFavoriteCurrencies(IEnumerable<string> currencyCodes)
    {
        if (currencyCodes == null)
            throw new ArgumentNullException(nameof(currencyCodes));
        
        var normalizedCodes = currencyCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim().ToUpperInvariant())
            .Distinct()
            .ToList();
        
        FavoriteCurrencies.Clear();
        FavoriteCurrencies.AddRange(normalizedCodes);
        UpdateTimestamp();
    }
}