using BCrypt.Net;
using FSM.Application.Interfaces;

namespace FSM.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // BCrypt otomatik olarak "Salt" üretir ve şifreyi karmaşık bir Hash'e çevirir.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // Gelen açık metin şifre ile veritabanındaki Hash'lenmiş şifreyi karşılaştırır.
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}