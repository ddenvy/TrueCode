using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrueCode.Finance.Application.Interfaces;
using UserGrpc = TrueCode.User.API.Grpc;

namespace TrueCode.Finance.Infrastructure.Services;

/// <summary>
/// Сервис для взаимодействия с пользовательским микросервисом через gRPC
/// </summary>
public class GrpcUserService : IUserService
{
    private readonly UserGrpc.UserService.UserServiceClient _userServiceClient;
    private readonly ILogger<GrpcUserService> _logger;
    private readonly string _userServiceUrl;

    public GrpcUserService(IConfiguration configuration, ILogger<GrpcUserService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _userServiceUrl = configuration["Services:UserService:GrpcUrl"] 
            ?? throw new InvalidOperationException("UserService gRPC URL не настроен");

        // Создаем gRPC канал
        var channel = GrpcChannel.ForAddress(_userServiceUrl);
        _userServiceClient = new UserGrpc.UserService.UserServiceClient(channel);
    }

    /// <inheritdoc />
    public async Task<UserInfoDto?> GetUserWithFavoriteCurrenciesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Запрос информации о пользователе {UserId} через gRPC", userId);

            var request = new UserGrpc.GetUserRequest
            {
                UserId = userId.ToString()
            };

            var response = await _userServiceClient.GetUserAsync(request, cancellationToken: cancellationToken);

            if (!response.Success || response.User == null)
            {
                _logger.LogWarning("Пользователь {UserId} не найден через gRPC: {Message}", userId, response.Message);
                return null;
            }

            var userInfo = new UserInfoDto(
                Guid.Parse(response.User.Id),
                response.User.Name,
                response.User.FavoriteCurrencies.ToList()
            );

            _logger.LogDebug("Получена информация о пользователе {UserId}: {UserName}, избранных валют: {Count}", 
                userId, userInfo.Name, userInfo.FavoriteCurrencies.Count);

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении информации о пользователе {UserId} через gRPC", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Проверка существования пользователя {UserId} через gRPC", userId);

            var request = new UserGrpc.GetUserRequest
            {
                UserId = userId.ToString()
            };

            var response = await _userServiceClient.GetUserAsync(request, cancellationToken: cancellationToken);
            
            var exists = response.Success && response.User != null;
            
            _logger.LogDebug("Пользователь {UserId} существует: {Exists}", userId, exists);
            
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке существования пользователя {UserId} через gRPC", userId);
            return false; // В случае ошибки считаем, что пользователь не существует
        }
    }
} 