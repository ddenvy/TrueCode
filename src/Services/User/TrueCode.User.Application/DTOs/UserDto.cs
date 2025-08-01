namespace TrueCode.User.Application.DTOs;

/// <summary>
/// DTO для представления пользователя
/// </summary>
public class UserDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Список избранных валют
    /// </summary>
    public List<string> FavoriteCurrencies { get; set; } = new();
    
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO для регистрации пользователя
/// </summary>
public class RegisterUserDto
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO для входа пользователя
/// </summary>
public class LoginUserDto
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO для ответа при аутентификации
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT токен
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public UserDto User { get; set; } = new();
}

/// <summary>
/// DTO для обновления избранных валют
/// </summary>
public class UpdateFavoriteCurrenciesDto
{
    /// <summary>
    /// Список кодов валют
    /// </summary>
    public List<string> CurrencyCodes { get; set; } = new();
}