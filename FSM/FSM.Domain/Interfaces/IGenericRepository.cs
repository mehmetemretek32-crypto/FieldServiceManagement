using FSM.Domain.Common;
using System.Linq.Expressions;

namespace FSM.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task UpdateAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    // ID dışında özel şartlarla arama yapmak için eklendi
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

    // Asıl tetiği çekecek olan metodumuz (Fırını çalıştırır)
    Task<int> SaveChangesAsync();
}