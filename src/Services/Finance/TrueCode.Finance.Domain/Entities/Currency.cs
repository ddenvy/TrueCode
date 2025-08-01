using TrueCode.Shared.Domain.Entities;

namespace TrueCode.Finance.Domain.Entities;

/// <summary>
/// Доменная модель валюты
/// </summary>
public class Currency : BaseEntity
{
    /// <summary>
    /// Название валюты (код валюты, например USD, EUR)
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Курс валюты к рублю
    /// </summary>
    public decimal Rate { get; private set; }
    
    /// <summary>
    /// Полное название валюты (например, "Доллар США")
    /// </summary>
    public string FullName { get; private set; } = string.Empty;
    
    /// <summary>
    /// Номинал валюты (например, для японской йены может быть 100)
    /// </summary>
    public int Nominal { get; private set; } = 1;
    
    /// <summary>
    /// Приватный конструктор для Entity Framework
    /// </summary>
    private Currency() { }
    
    /// <summary>
    /// Создает новую валюту
    /// </summary>
    /// <param name="name">Код валюты (USD, EUR, etc.)</param>
    /// <param name="rate">Курс к рублю</param>
    /// <param name="fullName">Полное название валюты</param>
    /// <param name="nominal">Номинал валюты</param>
    /// <returns>Новый экземпляр валюты</returns>
    public static Currency Create(string name, decimal rate, string fullName, int nominal = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Код валюты не может быть пустым", nameof(name));
        
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Название валюты не может быть пустым", nameof(fullName));
        
        if (rate <= 0)
            throw new ArgumentException("Курс валюты должен быть положительным", nameof(rate));
        
        if (nominal <= 0)
            throw new ArgumentException("Номинал валюты должен быть положительным", nameof(nominal));
        
        return new Currency
        {
            Name = name.Trim().ToUpperInvariant(),
            Rate = rate,
            FullName = fullName.Trim(),
            Nominal = nominal
        };
    }
    
    /// <summary>
    /// Обновляет курс валюты
    /// </summary>
    /// <param name="newRate">Новый курс</param>
    public void UpdateRate(decimal newRate)
    {
        if (newRate <= 0)
            throw new ArgumentException("Курс валюты должен быть положительным", nameof(newRate));
        
        Rate = newRate;
        UpdateTimestamp();
    }
    
    /// <summary>
    /// Обновляет информацию о валюте
    /// </summary>
    /// <param name="newRate">Новый курс</param>
    /// <param name="newFullName">Новое полное название</param>
    /// <param name="newNominal">Новый номинал</param>
    public void UpdateInfo(decimal newRate, string newFullName, int newNominal)
    {
        if (newRate <= 0)
            throw new ArgumentException("Курс валюты должен быть положительным", nameof(newRate));
        
        if (string.IsNullOrWhiteSpace(newFullName))
            throw new ArgumentException("Название валюты не может быть пустым", nameof(newFullName));
        
        if (newNominal <= 0)
            throw new ArgumentException("Номинал валюты должен быть положительным", nameof(newNominal));
        
        Rate = newRate;
        FullName = newFullName.Trim();
        Nominal = newNominal;
        UpdateTimestamp();
    }
} 