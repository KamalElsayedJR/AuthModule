using CORE.Interfaces;
using Microsoft.EntityFrameworkCore;
using REPOSITORY.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORY.Repsitories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IdentityDbContext _dbContext;

        public GenericRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(T item)
        => await _dbContext.Set<T>().AddAsync(item);
        public void Delete(T item)
        => _dbContext.Remove(item);
        public async Task<T?> GetByIdAsync(string id)
        => await _dbContext.FindAsync<T>(id);
        public void Update(T item)
        => _dbContext.Set<T>().Update(item);
    }
}
