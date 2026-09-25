using System.Linq.Expressions;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Common;

namespace NunyFoodWebApi.Tests.Support;

/// <summary>Données partagées par tous les repositories d'un même <see cref="TestApp"/>.</summary>
public sealed class InMemoryStore
{
    private readonly Dictionary<Type, object> _sets = [];

    public List<T> Set<T>() where T : BaseEntity
    {
        if (!_sets.TryGetValue(typeof(T), out var set))
            _sets[typeof(T)] = set = new List<T>();
        return (List<T>)set;
    }
}

/// <summary>
/// Remplace le repository EF : les écritures sont immédiates et, comme EF, Add attribue Id et CreatedAt.
/// </summary>
public sealed class InMemoryRepository<T>(InMemoryStore store) : IRepository<T> where T : BaseEntity
{
    private List<T> Items => store.Set<T>();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(e => e.Id == id));

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<T>>(Items.ToList());

    public Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<T>>(Items.Where(predicate.Compile()).ToList());

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(predicate.Compile()));

    public void Add(T entity)
    {
        if (entity.Id == Guid.Empty) entity.Id = Guid.CreateVersion7();
        if (entity.CreatedAt == default) entity.CreatedAt = DateTime.UtcNow;
        Items.Add(entity);
    }

    public void Remove(T entity) => Items.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => Task.FromResult(0);
}
