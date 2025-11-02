using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;

        private readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await dbSet.AddAsync(entity, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            dbSet.Remove(entity);

            await context.SaveChangesAsync(cancellationToken);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task<bool> EntityExists(Expression<Func<T, bool>> expression, CancellationToken cancellationToken)
        {
            return await dbSet.AnyAsync(expression, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken)
        {
            return await dbSet
                .AsNoTracking()
                .Where(expression)
                .ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await dbSet.FindAsync(id, cancellationToken);
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> expression)
        {
            return dbSet
                .AsNoTracking()
                .Where(expression);
        }

        public IQueryable<T> Query()
        {
            return dbSet;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            dbSet.Update(entity);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
