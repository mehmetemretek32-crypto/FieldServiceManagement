using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IGenericRepository<AppUser> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    // Yine sadece senin sisteminde var olan interfaceler
    public LoginCommandHandler(
        IGenericRepository<AppUser> userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Kullanıcıyı bul
        var user = await _userRepository.GetAsync(u => u.Email == request.Email);

        // 2. Kullanıcı yoksa veya şifre (Hash üzerinden) yanlışsa
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new Exception("E-posta adresi veya şifre hatalı.");
        }

        // 3. Her şey doğruysa Token'ı üret ve dön
        var token = _tokenService.GenerateToken(user);

        return token;
    }
}