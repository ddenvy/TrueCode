using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace TrueCode.User.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация Entity Framework для сущности пользователя
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<Domain.Entities.User>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.User> builder)
    {
        // Настройка таблицы
        builder.ToTable("users");
        
        // Настройка первичного ключа
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // Guid генерируется в доменной модели
        
        // Настройка имени пользователя
        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
        
        // Создание уникального индекса для имени пользователя
        builder.HasIndex(u => u.Name)
            .IsUnique()
            .HasDatabaseName("ix_users_name_unique");
        
        // Настройка пароля
        builder.Property(u => u.Password)
            .HasColumnName("password")
            .HasMaxLength(255)
            .IsRequired();
        
        // Настройка избранных валют как JSON
        builder.Property(u => u.FavoriteCurrencies)
            .HasColumnName("favorite_currencies")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );
        
        // Настройка временных меток
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}