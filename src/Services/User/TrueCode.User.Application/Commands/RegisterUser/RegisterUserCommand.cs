using MediatR;
using TrueCode.User.Application.DTOs;

namespace TrueCode.User.Application.Commands.RegisterUser;

/// <summary>
/// Команда для регистрации нового пользователя
/// </summary>
public class RegisterUserCommand : IRequest<AuthResponseDto>
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    public RegisterUserCommand(string name, string password)
    {
        Name = name;
        Password = password;
    }
}