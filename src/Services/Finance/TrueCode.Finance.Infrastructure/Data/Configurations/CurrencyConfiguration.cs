using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrueCode.Finance.Domain.Entities;

namespace TrueCode.Finance.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация Entity Framework для сущности валюты
/// </summary>
public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        // Настройка таблицы
        builder.ToTable("currency");
        
        // Настройка первичного ключа
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // Guid генерируется в доменной модели
        
        // Настройка кода валюты (name)
        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(10)
            .IsRequired();
        
        // Создание уникального индекса для кода валюты
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("ix_currency_name_unique");
        
        // Настройка курса валюты
        builder.Property(c => c.Rate)
            .HasColumnName("rate")
            .HasColumnType("decimal(18,4)")
            .IsRequired();
        
        // Настройка полного названия валюты
        builder.Property(c => c.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(255)
            .IsRequired();
        
        // Настройка номинала
        builder.Property(c => c.Nominal)
            .HasColumnName("nominal")
            .IsRequired()
            .HasDefaultValue(1);
        
        // Настройка временных меток
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
} 