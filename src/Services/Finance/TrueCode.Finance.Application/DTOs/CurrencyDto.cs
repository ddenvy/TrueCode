namespace TrueCode.Finance.Application.DTOs;

/// <summary>
/// DTO для валюты
/// </summary>
public record CurrencyDto(
    Guid Id,
    string Name,
    decimal Rate,
    string FullName,
    int Nominal,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// DTO для получения курсов валют по пользователю
/// </summary>
public record UserCurrencyRatesDto(
    Guid UserId,
    string UserName,
    List<CurrencyDto> FavoriteCurrencies,
    DateTime LastUpdated
);

/// <summary>
/// DTO для создания/обновления валюты
/// </summary>
public record CreateCurrencyDto(
    string Name,
    decimal Rate,
    string FullName,
    int Nominal = 1
);

/// <summary>
/// DTO для обновления курса валюты
/// </summary>
public record UpdateCurrencyRateDto(
    string Name,
    decimal Rate
);

/// <summary>
/// DTO для массового обновления курсов валют
/// </summary>
public record BulkUpdateCurrencyRatesDto(
    List<UpdateCurrencyRateDto> Currencies
); 