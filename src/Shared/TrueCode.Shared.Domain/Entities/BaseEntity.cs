namespace TrueCode.Shared.Domain.Entities;

/// <summary>
/// Базовая сущность с идентификатором
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Уникальный идентификатор сущности
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();
    
    /// <summary>
    /// Дата создания сущности
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Дата последнего обновления сущности
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Обновляет временную метку последнего изменения
    /// </summary>
    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}