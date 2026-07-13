using FSM.Domain.Common;

namespace FSM.Domain.Entities;

public class Customer : BaseEntity, ISoftDeletable
{
    // Kişisel / Kurumsal Bilgiler
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }

    // İletişim ve Adres
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;

    // Müşteriyi de veritabanından kalıcı silmek yerine pasife çekmek için:
    public bool IsDeleted { get; set; } = false;

    // Bire Çok İlişki (Bir müşterinin birden fazla iş emri olabilir)
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}