using FSM.Domain.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        await _context.SaveChangesAsync();
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

    public async Task UpdateAsync(T entity)
    {
        // FindAsync ile gelen entity zaten izleniyor (tracked). Update() tüm navigation
        // property'leri Modified yapar ve DbUpdateConcurrencyException tetikleyebilir.
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _dbSet.Update(entity);
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        // Veritabanına "Sadece benim gönderdiğim şarta uyan İLK kaydı getir" diyoruz.
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    // 🔥 PRO DOKUNUŞ: Sadece InventoryItem için değil, tüm sınıflar (T) için çalışan asenkron silme metodu!
    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask; // Remove işlemi Entity Framework'te asenkron olmadığı için Task.CompletedTask dönüyoruz.
    }

    public IQueryable<T> GetAllAsQueryable()
    {
        // DbSet'i IQueryable olarak dışarı açıyoruz ki Handler içinde Include yapabilelim
        return _context.Set<T>().AsQueryable();
    }
}