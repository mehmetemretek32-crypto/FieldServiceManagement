using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
// Eğer şifreleme için BCrypt kullanıyorsan: using BCrypt.Net;

namespace FSM.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IGenericRepository<AppUser> _userRepository;

    public ChangePasswordCommandHandler(IGenericRepository<AppUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. Kullanıcıyı getir
        var user = await _userRepository.GetAsync(x => x.Id == request.UserId);

        if (user == null)
            throw new Exception("Kullanıcı bulunamadı.");

        // 2. Mevcut şifre doğru mu kontrol et?
        // NOT: Kendi projende şifre doğrulamayı nasıl yapıyorsan burayı ona göre uyarla.
        // Örnek BCrypt kullanımı:
        bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);

        if (!isCurrentPasswordValid)
            throw new Exception("Mevcut şifrenizi yanlış girdiniz.");

        // 3. Yeni şifreyi Hash'le (Şifrele)
        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        // 4. Kullanıcının şifresini güncelle ve kaydet
        user.PasswordHash = newPasswordHash;

        await _userRepository.UpdateAsync(user);

        return true;
    }
}