using Microsoft.EntityFrameworkCore;
using TrueCode.Shared.Infrastructure.Repositories;
using TrueCode.User.Domain.Interfaces;
using TrueCode.User.Infrastructure.Data;

namespace TrueCode.User.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория для работы с пользователями
/// </summary>
public class UserRepository : BaseRepository<Domain.Entities.User>, IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Domain.Entities.User?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return await _context.Users
            .FirstOrDefaultAsync(u => u.Name == name.Trim(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return await _context.Users
            .AnyAsync(u => u.Name == name.Trim(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name, Guid excludeUserId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return await _context.Users
            .AnyAsync(u => u.Name == name.Trim() && u.Id != excludeUserId, cancellationToken);
    }
}