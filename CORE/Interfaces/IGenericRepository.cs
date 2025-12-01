using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    public interface IGenericRepository<T> where T : class 
    {
        Task<T?> GetByIdAsync(string id);
        
        Task AddAsync(T item);
        void Update(T item);
        void Delete(T item);
    }
}
