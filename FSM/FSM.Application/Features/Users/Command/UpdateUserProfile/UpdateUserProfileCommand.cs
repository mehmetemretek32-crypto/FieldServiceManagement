using MediatR;

namespace FSM.Application.Features.Users.Command.UpdateUSerProfile;

// Kayıt başarılıysa geriye bool döneceğiz.
public record UpdateUserProfileCommand(
    int UserId, // Bu ID'yi URL'den veya doğrudan JWT Token içinden alacağız
    string FirstName,
    string LastName,
    string Email
) : IRequest<bool>;