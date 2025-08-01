using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TrueCode.Shared.Infrastructure.Services;

/// <summary>
/// Сервис для работы с JWT токенами
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Генерирует JWT токен для пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="userName">Имя пользователя</param>
    /// <returns>JWT токен</returns>
    string GenerateToken(Guid userId, string userName);
    
    /// <summary>
    /// Валидирует JWT токен
    /// </summary>
    /// <param name="token">JWT токен</param>
    /// <returns>ClaimsPrincipal если токен валиден, иначе null</returns>
    ClaimsPrincipal? ValidateToken(string token);
    
    /// <summary>
    /// Извлекает идентификатор пользователя из токена
    /// </summary>
    /// <param name="token">JWT токен</param>
    /// <returns>Идентификатор пользователя</returns>
    Guid? GetUserIdFromToken(string token);
}

/// <summary>
/// Реализация сервиса для работы с JWT токенами
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey не настроен");
        _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer не настроен");
        _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience не настроен");
        _expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
    }

    /// <inheritdoc />
    public string GenerateToken(Guid userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Имя пользователя не может быть пустым", nameof(userName));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public Guid? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal == null)
            return null;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return null;

        return userId;
    }
}