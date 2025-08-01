using Microsoft.EntityFrameworkCore;
using TrueCode.User.Infrastructure.Data.Configurations;

namespace TrueCode.User.Infrastructure.Data;

/// <summary>
/// Контекст базы данных для работы с пользователями
/// </summary>
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Набор пользователей
    /// </summary>
    public DbSet<Domain.Entities.User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Применяем конфигурации сущностей
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}