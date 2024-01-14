using Store.Core;
using Store.Core.Entities;
using Store.Core.Repository;
using Store.Repository.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly StoreDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        private bool _disposed = false;

        public UnitOfWork(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CompleteAsync() => await _dbContext.SaveChangesAsync();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var typeEntity = typeof(TEntity);
            if (!_repositories.TryGetValue(typeEntity, out var repository))
            {
                repository = new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(typeEntity, repository);
            }
            return (IGenericRepository<TEntity>)repository;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _dbContext.DisposeAsync();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        public void Dispose() => DisposeAsync().GetAwaiter().GetResult();
    }
}