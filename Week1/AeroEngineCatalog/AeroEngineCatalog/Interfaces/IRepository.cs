using System.Linq.Expressions;

namespace AeroEngineCatalog.Interfaces;

public interface IRepository<T> where T : class
{
    // Асинхронные CRUD операции
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    
    // Утилиты
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}