using BCrypt.Net;

namespace TrueCode.Shared.Infrastructure.Services;

/// <summary>
/// Интерфейс сервиса для работы с хешированием паролей
/// </summary>
public interface IPasswordHashService
{
    /// <summary>
    /// Хеширует пароль
    /// </summary>
    /// <param name="password">Пароль для хеширования</param>
    /// <returns>Хешированный пароль</returns>
    string HashPassword(string password);
    
    /// <summary>
    /// Проверяет соответствие пароля хешу
    /// </summary>
    /// <param name="password">Пароль для проверки</param>
    /// <param name="hashedPassword">Хешированный пароль</param>
    /// <returns>True, если пароль соответствует хешу</returns>
    bool VerifyPassword(string password, string hashedPassword);
}

/// <summary>
/// Сервис для хеширования и проверки паролей с использованием BCrypt
/// </summary>
public class PasswordHashService : IPasswordHashService
{
    private const int WorkFactor = 12;

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Пароль не может быть пустым", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Пароль не может быть пустым", nameof(password));
        
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Хешированный пароль не может быть пустым", nameof(hashedPassword));

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }
}