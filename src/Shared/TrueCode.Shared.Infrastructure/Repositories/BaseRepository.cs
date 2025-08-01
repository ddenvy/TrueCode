using Microsoft.EntityFrameworkCore;
using TrueCode.Shared.Domain.Entities;
using TrueCode.Shared.Domain.Interfaces;

namespace TrueCode.Shared.Infrastructure.Repositories;

/// <summary>
/// Базовая реализация репозитория с использованием Entity Framework
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DbContext Context;
    protected readonly DbSet<T> DbSet;

    protected BaseRepository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var entry = await DbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Update(entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }
}