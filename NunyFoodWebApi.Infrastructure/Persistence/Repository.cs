using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Common;

namespace NunyFoodWebApi.Infrastructure.Persistence;

public class Repository<T>(NunyFoodDbContext context) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _set.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await _set.ToListAsync(ct);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _set.Where(predicate).ToListAsync(ct);

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _set.FirstOrDefaultAsync(predicate, ct);

    public void Add(T entity) => _set.Add(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        context.SaveChangesAsync(ct);
}
