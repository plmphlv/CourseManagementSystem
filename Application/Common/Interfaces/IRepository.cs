using System.Linq.Expressions;

namespace Application.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<bool> EntityExists(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);

        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        Task UpdateAsync(T entity, CancellationToken cancellationToken);

        Task UpdateAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        Task DeleteAsync(T entity, CancellationToken cancellationToken);

        void DeleteRange(IEnumerable<T> entities);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);

        IQueryable<T> Query(Expression<Func<T, bool>> expression);

        IQueryable<T> Query();
    }
}
