using MediatR;
using TrueCode.User.Application.DTOs;

namespace TrueCode.User.Application.Queries.GetUser;

/// <summary>
/// Запрос для получения информации о пользователе
/// </summary>
public class GetUserQuery : IRequest<UserDto?>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }
    
    public GetUserQuery(Guid userId)
    {
        UserId = userId;
    }
}