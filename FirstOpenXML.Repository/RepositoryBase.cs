using FirstOpenXML.Contracts;
using FirstOpenXML.Core.Database;
using FirstOpenXML.Core.Entities;
using Microsoft.EntityFrameworkCore;
using SmartZone.Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirstOpenXML.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(AppDbContext repositoryContext)
        {
            _context = repositoryContext;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entities) => _dbSet.Add(entities);

        public void AddRange(IEnumerable<T> entities) => _dbSet.AddRange(entities);

        public virtual void Delete(T entity) => _dbSet.Remove(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public virtual IQueryable<T> FindAll(Expression<Func<T, bool>>? predicate = null)
             => _dbSet.WhereIf(predicate != null, predicate!);

        public virtual async Task<T?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(id);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
    }
}
