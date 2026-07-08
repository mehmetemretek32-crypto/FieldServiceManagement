using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces; // Senin IGenericRepository arayüzünün olduğu yer

namespace FSM.Application.Features.Dashboard.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IGenericRepository<WorkOrder> _workOrderRepository;
        private readonly IGenericRepository<Technician> _technicianRepository;

        public GetDashboardStatsQueryHandler(
            IGenericRepository<WorkOrder> workOrderRepository,
            IGenericRepository<Technician> technicianRepository)
        {
            _workOrderRepository = workOrderRepository;
            _technicianRepository = technicianRepository;
        }

        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            // Senin arabiriminde var olan GetAllAsync() ile verileri çekiyoruz
            var workOrders = await _workOrderRepository.GetAllAsync();
            var technicians = await _technicianRepository.GetAllAsync();

            // Bellekte sayısal hesaplamayı yapıp DTO olarak dönüyoruz
            return new DashboardStatsDto
            {
                TotalWorkOrders = workOrders.Count(),

                ActiveTechnicians = technicians.Count(),

                // Enum int değeri 0 olanlar (Pending / Beklemede)
                PendingAssignments = workOrders.Count(w => (int)w.State == 0),

                // Enum int değeri 2 olanlar (Completed / Tamamlandı)
                CompletedJobs = workOrders.Count(w => (int)w.State == 2)
            };
        }
    }
}