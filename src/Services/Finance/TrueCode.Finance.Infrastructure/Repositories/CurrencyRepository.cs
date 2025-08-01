using Microsoft.EntityFrameworkCore;
using TrueCode.Finance.Domain.Entities;
using TrueCode.Finance.Domain.Interfaces;
using TrueCode.Finance.Infrastructure.Data;

namespace TrueCode.Finance.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория для работы с валютами
/// </summary>
public class CurrencyRepository : ICurrencyRepository
{
    private readonly FinanceDbContext _context;

    public CurrencyRepository(FinanceDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Currency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Currency?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalizedName = name.Trim().ToUpperInvariant();
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.Name == normalizedName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Currencies
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Currency>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        if (names == null || !names.Any())
            return new List<Currency>();

        var normalizedNames = names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim().ToUpperInvariant())
            .Distinct()
            .ToList();

        if (!normalizedNames.Any())
            return new List<Currency>();

        return await _context.Currencies
            .Where(c => normalizedNames.Contains(c.Name))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Currency currency, CancellationToken cancellationToken = default)
    {
        if (currency == null)
            throw new ArgumentNullException(nameof(currency));

        await _context.Currencies.AddAsync(currency, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(IEnumerable<Currency> currencies, CancellationToken cancellationToken = default)
    {
        if (currencies == null)
            throw new ArgumentNullException(nameof(currencies));

        var currencyList = currencies.ToList();
        if (currencyList.Any())
        {
            await _context.Currencies.AddRangeAsync(currencyList, cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default)
    {
        if (currency == null)
            throw new ArgumentNullException(nameof(currency));

        _context.Currencies.Update(currency);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Currency currency, CancellationToken cancellationToken = default)
    {
        if (currency == null)
            throw new ArgumentNullException(nameof(currency));

        _context.Currencies.Remove(currency);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var normalizedName = name.Trim().ToUpperInvariant();
        return await _context.Currencies
            .AnyAsync(c => c.Name == normalizedName, cancellationToken);
    }
} 