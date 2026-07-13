using FSM.Domain.Common;
using FSM.Domain.Interfaces;

namespace FSM.Application.Common;

// Handler'larda tekrar eden "getir + kontrol et" kalıplarını tek yerde toplayan yardımcılar.
public static class RepositoryExtensions
{
    // ID'ye göre getirir; kayıt yoksa NotFoundException fırlatır.
    public static async Task<T> GetByIdOrThrowAsync<T>(
        this IGenericRepository<T> repository, int id, string entityName)
        where T : BaseEntity
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
            throw new NotFoundException($"ID'si {id} olan {entityName} bulunamadı.");

        return entity;
    }

    // ID'ye göre getirir; kayıt yoksa veya pasife çekilmişse NotFoundException fırlatır.
    public static async Task<T> GetActiveByIdOrThrowAsync<T>(
        this IGenericRepository<T> repository, int id, string entityName)
        where T : BaseEntity, ISoftDeletable
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null || entity.IsDeleted)
            throw new NotFoundException($"ID'si {id} olan aktif bir {entityName} bulunamadı!");

        return entity;
    }

    // Ortak soft-delete akışı: getir, yoksa/zaten silinmişse hata ver, pasife çek ve kaydet.
    public static async Task SoftDeleteAsync<T>(
        this IGenericRepository<T> repository, int id, string entityName)
        where T : BaseEntity, ISoftDeletable
    {
        var entity = await repository.GetByIdAsync(id);

        if (entity is null)
            throw new NotFoundException($"Hata: ID'si {id} olan {entityName} bulunamadı.");

        if (entity.IsDeleted)
            throw new NotFoundException($"Hata: ID'si {id} olan {entityName} zaten daha önceden silinmiş!");

        entity.IsDeleted = true;

        await repository.UpdateAsync(entity);
        await repository.SaveChangesAsync();
    }
}
