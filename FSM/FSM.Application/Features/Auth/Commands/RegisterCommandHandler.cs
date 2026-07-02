using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    private readonly IGenericRepository<AppUser> _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    // Constructor (Dependency Injection)
    public RegisterCommandHandler(IGenericRepository<AppUser> userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. E-posta adresi zaten kullanılıyor mu kontrolü
        var existingUser = await _userRepository.GetAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            throw new Exception("Bu e-posta adresi zaten sistemde kayıtlı.");
        }

        // 2. Yeni kullanıcı nesnesini oluştur (Tek bir newUser bloğu)
        var newUser = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password), // Şifre burada Hash'leniyor!
            Role = request.Role
        };

        // 3. Repository üzerinden kaydet
        await _userRepository.AddAsync(newUser);

        return true;
    }
}