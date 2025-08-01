using Microsoft.EntityFrameworkCore;
using TrueCode.Finance.Domain.Entities;
using TrueCode.Finance.Infrastructure.Data.Configurations;

namespace TrueCode.Finance.Infrastructure.Data;

/// <summary>
/// Контекст базы данных для работы с финансовыми данными
/// </summary>
public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Набор валют
    /// </summary>
    public DbSet<Currency> Currencies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Применяем конфигурации сущностей
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
    }
} 