using Store.Core.Entities;
using Store.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<int> CompleteAsync();

        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

    }
}
