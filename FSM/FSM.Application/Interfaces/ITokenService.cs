using FSM.Domain.Entities;

namespace FSM.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(AppUser user);
}