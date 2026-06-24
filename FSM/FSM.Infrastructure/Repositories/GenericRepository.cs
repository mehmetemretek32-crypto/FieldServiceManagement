using FSM.Domain.Common;
using FSM.Domain.Interfaces;
using FSM.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FSM.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    // Bekleyen tüm Ekle/Sil/Güncelle işlemlerini topluca SQL'e yazar
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}