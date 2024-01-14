using Store.Core.Entities;
using Store.Core.Specification;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Store.Core.Repository
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<T> GetEntityWithSpecAsync(ISpecification<T> spec);
        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);   

        Task AddAsync(T Entity);
        void Delete(T Entity);
        void Update(T Entity);
        public Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    }
}
