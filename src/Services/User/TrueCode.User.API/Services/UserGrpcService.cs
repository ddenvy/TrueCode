using Grpc.Core;
using MediatR;
using TrueCode.Shared.Infrastructure.Services;
using TrueCode.User.Application.Queries.GetUser;
using TrueCode.User.API.Grpc;

namespace TrueCode.User.API.Services;

/// <summary>
/// gRPC сервис для работы с пользователями
/// </summary>
public class UserGrpcService : UserService.UserServiceBase
{
    private readonly IMediator _mediator;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UserGrpcService> _logger;

    public UserGrpcService(
        IMediator mediator,
        IJwtService jwtService,
        ILogger<UserGrpcService> logger)
    {
        _mediator = mediator;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// Получение информации о пользователе
    /// </summary>
    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return new GetUserResponse
                {
                    Success = false,
                    Message = "Неверный формат идентификатора пользователя"
                };
            }

            var query = new GetUserQuery(userId);
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return new GetUserResponse
                {
                    Success = false,
                    Message = "Пользователь не найден"
                };
            }

            return new GetUserResponse
            {
                Success = true,
                Message = "Пользователь найден",
                User = new UserInfo
                {
                    Id = user.Id.ToString(),
                    Name = user.Name,
                    CreatedAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    UpdatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении пользователя {UserId}", request.UserId);
            return new GetUserResponse
            {
                Success = false,
                Message = "Внутренняя ошибка сервера"
            };
        }
    }

    /// <summary>
    /// Валидация JWT токена
    /// </summary>
    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        try
        {
            var principal = _jwtService.ValidateToken(request.Token);
            
            if (principal == null)
            {
                return new ValidateTokenResponse
                {
                    IsValid = false,
                    UserId = string.Empty,
                    UserName = string.Empty,
                    Message = "Недействительный токен"
                };
            }

            var userIdClaim = principal.FindFirst("userId")?.Value;
            var userNameClaim = principal.FindFirst("userName")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return new ValidateTokenResponse
                {
                    IsValid = false,
                    UserId = string.Empty,
                    UserName = string.Empty,
                    Message = "Неверный формат токена"
                };
            }

            // Проверяем, что пользователь существует
            var query = new GetUserQuery(userId);
            var user = await _mediator.Send(query);

            return new ValidateTokenResponse
            {
                IsValid = user != null,
                UserId = user?.Id.ToString() ?? string.Empty,
                UserName = user?.Name ?? userNameClaim ?? string.Empty,
                Message = user != null ? "Токен действителен" : "Пользователь не найден"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при валидации токена");
            return new ValidateTokenResponse
            {
                IsValid = false,
                UserId = string.Empty,
                UserName = string.Empty,
                Message = "Внутренняя ошибка сервера"
            };
        }
    }
}