using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TrueCode.User.Infrastructure.Data;

/// <summary>
/// Фабрика для создания UserDbContext во время дизайна (для миграций)
/// </summary>
public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
        
        // Используем строку подключения по умолчанию для миграций
        var connectionString = "Host=localhost;Port=5432;Database=truecode_user_dev;Username=postgres;Password=postgres";
        
        optionsBuilder.UseNpgsql(connectionString);
        
        return new UserDbContext(optionsBuilder.Options);
    }
}