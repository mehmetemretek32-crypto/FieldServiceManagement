using FSM.Domain.Common;

namespace FSM.Application.Common;

public static class SoftDeleteExtensions
{
    // Pasife çekilmemiş (aktif) kayıtları döndürür.
    public static IEnumerable<T> OnlyActive<T>(this IEnumerable<T> source)
        where T : ISoftDeletable
    {
        return source.Where(x => !x.IsDeleted);
    }
}
