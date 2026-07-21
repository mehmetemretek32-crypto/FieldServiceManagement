using FSM.Application.Features.Users.Command.UpdateUSerProfile;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
// Kendi repository interface'inin bulunduğu namespace'i buraya eklemelisin
// using FSM.Application.Interfaces.Repositories; 

namespace FSM.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IGenericRepository<AppUser> _userRepository;

    public UpdateUserProfileCommandHandler(IGenericRepository<AppUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        // 1. Kullanıcıyı veritabanından getir
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
            throw new Exception("Güncellenmek istenen kullanıcı bulunamadı."); // İleride bunu NotFoundException olarak değiştirebiliriz.

        // 2. Yeni e-posta adresi başka birine mi ait kontrolü (Eğer e-posta değişiyorsa)
    
        if (user.Email != request.Email)
        {
            // AnyAsync yerine, o e-postaya sahip bir kullanıcı var mı diye veritabanından çekip kontrol edelim
            var existingUser = await _userRepository.GetAsync(x => x.Email == request.Email);

            if (existingUser != null) // Eğer o e-postayla biri bulunduysa
                throw new Exception("Bu e-posta adresi sistemde zaten kayıtlı.");
        }

        // 3. Verileri güncelle
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        // BaseEntity'de UpdatedDate gibi bir alanın varsa burada güncelleyebilirsin
        // user.UpdatedDate = DateTime.UtcNow;

        // 4. Veritabanına kaydet
        await _userRepository.UpdateAsync(user);

        return true;
    }
}