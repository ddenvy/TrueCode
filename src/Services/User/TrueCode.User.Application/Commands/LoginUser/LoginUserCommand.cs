using MediatR;
using TrueCode.User.Application.DTOs;

namespace TrueCode.User.Application.Commands.LoginUser;

/// <summary>
/// Команда для входа пользователя в систему
/// </summary>
public class LoginUserCommand : IRequest<AuthResponseDto>
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    public LoginUserCommand(string name, string password)
    {
        Name = name;
        Password = password;
    }
}