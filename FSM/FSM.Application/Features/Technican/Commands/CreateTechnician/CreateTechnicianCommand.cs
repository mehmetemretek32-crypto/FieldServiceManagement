using MediatR;

namespace FSM.Application.Features.Technican.Commands.CreateTechnician;
// Teknisyen eklendikten sonra geriye yeni oluşan ID'yi (int) döndüreceğiz
public record CreateTechnicianCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsAvailable = true
) : IRequest<int>;