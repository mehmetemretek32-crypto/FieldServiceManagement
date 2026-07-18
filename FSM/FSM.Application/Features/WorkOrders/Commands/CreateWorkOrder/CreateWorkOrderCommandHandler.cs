using AutoMapper;
using FluentValidation;
using FSM.Application.Common;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, int>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Customer> _customerRepository;
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IValidator<CreateWorkOrderCommand> _validator;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public CreateWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Customer> customerRepository,
        IGenericRepository<Technician> technicianRepository,
        IMapper mapper,
        INotificationService notificationService,
        IValidator<CreateWorkOrderCommand> validator,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _workOrderRepository = workOrderRepository;
        _customerRepository = customerRepository;
        _technicianRepository = technicianRepository;
        _mapper = mapper;
        _notificationService = notificationService;
        _validator = validator;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<int> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _customerRepository.GetActiveByIdOrThrowAsync(request.CustomerId, "müşteri");

        string technicianName = "Atanmadı";
        if (request.TechnicianId.HasValue && request.TechnicianId.Value > 0)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId.Value);
            if (technician == null || technician.IsDeleted)
            {
                throw new Exception($"Hata: ID'si {request.TechnicianId} olan aktif bir teknisyen bulunamadı!");
            }
            technicianName = technician.FullName;
        }

        var entity = _mapper.Map<WorkOrder>(request);
        entity.CreatedAt = DateTime.UtcNow;

        await _workOrderRepository.AddAsync(entity);
        await _workOrderRepository.SaveChangesAsync(); // Fırını Çalıştır! 🔥

        // 👇 🔥 ZİNCİRLEME ÇEKMECE TEMİZLİĞİ
        await _cache.RemoveAsync("all_work_orders_list", cancellationToken);
        await _cache.RemoveAsync("all_customers_list", cancellationToken); // Müşterinin iş sayısı arttı

        // Eğer bir teknisyen atandıysa onun da çekmecelerini temizle
        if (request.TechnicianId.HasValue && request.TechnicianId.Value > 0)
        {
            await _cache.RemoveAsync("all_technicians_list", cancellationToken);
            await _cache.RemoveAsync($"active_orders_for_tech_{request.TechnicianId.Value}", cancellationToken);
        }

        var customer = await _customerRepository.GetActiveByIdOrThrowAsync(request.CustomerId, "müşteri");
        await _notificationService.SendWorkOrderNotification($"#{entity.Id} no'lu yeni iş emri açıldı! Müşteri: {customer.FirstName} {customer.LastName} | Atanan: {technicianName}");

        return entity.Id;
    }
}