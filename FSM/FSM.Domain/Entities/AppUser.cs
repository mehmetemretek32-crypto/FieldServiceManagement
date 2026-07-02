using FSM.Domain.Common; 

namespace FSM.Domain.Entities;

// Eğer sisteminizde BaseEntity (Id, CreatedDate vs. tutan) varsa ondan miras al, yoksa direkt public class AppUser yazıp Id'yi kendin ekle.
public class AppUser : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Şifreleri ASLA açık metin (123456 gibi) tutmayız. Hash'lenmiş (şifrelenmiş) halini tutacağız.
    public string PasswordHash { get; set; } = string.Empty;

    // Kullanıcının sistemdeki rolü (Admin, Technician, Customer vb.)
    public string Role { get; set; } = "User";
}