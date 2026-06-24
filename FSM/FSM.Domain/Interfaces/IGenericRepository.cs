using FSM.Domain.Common;

namespace FSM.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    // Asıl tetiği çekecek olan metodumuz (Fırını çalıştırır)
    Task<int> SaveChangesAsync();
}