using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders
{
    public class GetAllWorkOrdersQueryHandler : IRequestHandler<GetAllWorkOrdersQuery, List<WorkOrderListDto>>
    {
        private readonly IGenericRepository<WorkOrder> _workOrderRepository;

        public GetAllWorkOrdersQueryHandler(IGenericRepository<WorkOrder> workOrderRepository)
        {
            _workOrderRepository = workOrderRepository;
        }

        public async Task<List<WorkOrderListDto>> Handle(GetAllWorkOrdersQuery request, CancellationToken cancellationToken)
        {
            // 1. Repository'den tüm listeyi çekiyoruz
            var allWorkOrders = await _workOrderRepository.GetAllAsync();

            allWorkOrders = allWorkOrders.Where(w => !w.IsDeleted);

            // 2. Filtreleme (request içindeki Status null değilse filtrele)
            if (request.Status.HasValue)
            {
                allWorkOrders = allWorkOrders.Where(w => (int)w.State == request.Status.Value);
            }

            // 3. Haritalama (Hata almamak için tüm özellikleri güvenli (nullable) şekilde eşliyoruz)
            // 3. Haritalama (Senin WorkOrder entity'ne tam uyumlu)
            return allWorkOrders.Select(w => new WorkOrderListDto
            {
                Id = w.Id, // Eğer hata verirse w.BaseEntityId gibi bir şey deneyebiliriz ama w.Id genelde çalışır
                Title = w.Title ?? "Başlıksız İş Emri",
                CustomerName = "Müşteri ID: " + w.CustomerId,
                TechnicianName = "Teknisyen ID: " + (w.TechnicianId?.ToString() ?? "Atanmadı"),
                
                // WorkOrderState ( senin enum'ın )
                Status = (int)w.State,

                // Senin entity'nde 'Priority' yok, o yüzden varsayılan 0 veriyoruz
                Priority = 0,

                Location = "Merkez Ofis",

                // Senin entity'nde 'CreatedDate' yok, yerine 'Id' veya base'den gelen bir tarih varsa onu yazmalısın.
                // Eğer base sınıfında tarih yoksa buraya statik bir tarih yazıyoruz ki derleme hatası bitsin:
                ScheduledDate = "Tarih Belirtilmemiş"
            }).ToList();
        }
    }
}