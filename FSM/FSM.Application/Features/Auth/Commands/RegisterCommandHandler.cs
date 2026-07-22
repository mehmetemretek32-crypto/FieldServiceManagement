using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    private readonly IGenericRepository<AppUser> _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    // Senin kendi var olan interfacelerin
    public RegisterCommandHandler(
        IGenericRepository<AppUser> userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. E-posta kontrolü
        var existingUser = await _userRepository.GetAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            throw new Exception("Bu e-posta adresi zaten sistemde kayıtlı.");
        }

        // 2. Kullanıcıyı oluştur
        var newUser = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = "Technician"
        };

        // 3. İzlemeye al
        await _userRepository.AddAsync(newUser);

        // 4. İŞTE BÜTÜN SORUNU ÇÖZEN SATIR: Senin repository'ndeki Save metodunu çağırıyoruz!
        await _userRepository.SaveChangesAsync();

        return true;
    }
}