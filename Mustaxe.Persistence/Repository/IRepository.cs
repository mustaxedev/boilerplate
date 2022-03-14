using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mustaxe.Persistence.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<TEntity> GetByIdAsync(int id);
        IQueryable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> UpdateAsync(TEntity entity);
    }
}