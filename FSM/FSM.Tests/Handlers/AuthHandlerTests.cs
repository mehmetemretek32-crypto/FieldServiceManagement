using System.Linq.Expressions;
using FSM.Application.Common;
using FSM.Application.Features.Auth.Commands;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using Moq;

namespace FSM.Tests.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IGenericRepository<AppUser>> _userRepository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    private LoginCommandHandler CreateHandler() =>
        new(_userRepository.Object, _tokenService.Object, _passwordHasher.Object);

    [Fact]
    public async Task Handle_ReturnsToken_WhenCredentialsAreValid()
    {
        var user = new AppUser { Email = "user@example.com", PasswordHash = "hashed" };
        _userRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.VerifyPassword("secret", "hashed")).Returns(true);
        _tokenService.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

        var result = await CreateHandler().Handle(
            new LoginCommand { Email = "user@example.com", Password = "secret" },
            CancellationToken.None);

        Assert.Equal("jwt-token", result);
        _tokenService.Verify(t => t.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenUserNotFound()
    {
        _userRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
            .ReturnsAsync((AppUser?)null);

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new LoginCommand { Email = "missing@example.com", Password = "secret" },
            CancellationToken.None));

        _tokenService.Verify(t => t.GenerateToken(It.IsAny<AppUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_WhenPasswordIsWrong()
    {
        var user = new AppUser { Email = "user@example.com", PasswordHash = "hashed" };
        _userRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new LoginCommand { Email = "user@example.com", Password = "wrong" },
            CancellationToken.None));

        _tokenService.Verify(t => t.GenerateToken(It.IsAny<AppUser>()), Times.Never);
    }
}

public class RegisterCommandHandlerTests
{
    private readonly Mock<IGenericRepository<AppUser>> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    private RegisterCommandHandler CreateHandler() =>
        new(_userRepository.Object, _passwordHasher.Object);

    [Fact]
    public async Task Handle_HashesPasswordAndAddsUser_WhenEmailIsNew()
    {
        _userRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
            .ReturnsAsync((AppUser?)null);
        _passwordHasher.Setup(h => h.HashPassword("secret")).Returns("hashed");

        AppUser? added = null;
        _userRepository
            .Setup(r => r.AddAsync(It.IsAny<AppUser>()))
            .Callback<AppUser>(u => added = u)
            .Returns(Task.CompletedTask);

        var result = await CreateHandler().Handle(new RegisterCommand
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "secret"
            
        }, CancellationToken.None);

        Assert.True(result);
        Assert.NotNull(added);
        Assert.Equal("hashed", added!.PasswordHash);
        Assert.Equal("jane@example.com", added.Email);
        
    }

    [Fact]
    public async Task Handle_Throws_WhenEmailAlreadyExists()
    {
        _userRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
            .ReturnsAsync(new AppUser { Email = "jane@example.com" });

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new RegisterCommand { Email = "jane@example.com", Password = "secret" },
            CancellationToken.None));

        _userRepository.Verify(r => r.AddAsync(It.IsAny<AppUser>()), Times.Never);
    }
}
