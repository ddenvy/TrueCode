using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TrueCode.Shared.Infrastructure.Services;
using TrueCode.User.API.Services;
using TrueCode.User.Application.Commands.RegisterUser;
using TrueCode.User.Infrastructure.Data;
using TrueCode.User.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// Добавляем Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TrueCode User API",
        Version = "v1",
        Description = "API для управления пользователями"
    });

    // Добавляем поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Включаем XML комментарии
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Добавляем MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

// Добавляем FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);

// Добавляем инфраструктуру пользователей
builder.Services.AddUserInfrastructure(builder.Configuration);

// Добавляем общие сервисы
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

// Настройка JWT аутентификации
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey не настроен");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Добавляем gRPC
builder.Services.AddGrpc();

// Добавляем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Добавляем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Применяем миграции при запуске
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    try
    {
        await context.Database.MigrateAsync();
        app.Logger.LogInformation("Миграции базы данных успешно применены");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Ошибка при применении миграций базы данных");
        throw;
    }
}

// Настройка pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrueCode User API v1");
        c.RoutePrefix = string.Empty; // Swagger UI на корневом пути
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Регистрируем gRPC сервисы
app.MapGrpcService<UserGrpcService>();

// Добавляем информацию о доступных эндпоинтах
app.MapGet("/", () => new
{
    Service = "TrueCode User API",
    Version = "1.0.0",
    Endpoints = new
    {
        REST = "/api/user",
        gRPC = "UserService",
        Swagger = "/swagger",
        Health = "/health"
    }
});

// Добавляем health check
app.MapGet("/health", async (UserDbContext dbContext) =>
{
    try
    {
        await dbContext.Database.CanConnectAsync();
        return Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 503,
            title: "Database connection failed");
    }
});

app.Logger.LogInformation("TrueCode User API запущен");
app.Logger.LogInformation("REST API доступен по адресу: /api/user");
app.Logger.LogInformation("gRPC сервис: UserService");
app.Logger.LogInformation("Swagger UI доступен по адресу: /");

app.Run();