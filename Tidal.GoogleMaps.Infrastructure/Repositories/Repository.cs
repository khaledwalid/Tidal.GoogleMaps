using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tidal.GoogleMaps.Contracts.Contracts;
using Tidal.GoogleMaps.DB.Context;
using Tidal.GoogleMaps.Models.Models;

namespace Tidal.GoogleMaps.Infrastructure.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : BaseModel
    {
        private readonly GoogleMapsDbContext _dbContext;

        public Repository(GoogleMapsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> expression)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            if (expression != null)
                query = query.Where(expression);

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                throw new ArgumentException("Expression is null");

            return await _dbContext.Set<T>().FirstOrDefaultAsync(expression);
        }

        public async Task<T> CreateAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreationDate = DateTime.UtcNow;
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity.Id == null)
                throw new ArgumentException("Id is null");
            await Task.FromResult(_dbContext.Set<T>().Update(entity));
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == null)
                throw new ArgumentException("Id is null");
            var entity = await GetAsync(a => a.Id == id);
            if(entity == null)
                throw new Exception("entity not found");
            await Task.FromResult(_dbContext.Set<T>().Remove(entity));
            await _dbContext.SaveChangesAsync();
        }

    }
}