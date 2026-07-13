using AutoMapper;
using FluentValidation;
using FSM.Application.Common;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, int>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Customer> _customerRepository;
    private readonly IGenericRepository<Technician> _technicianRepository; // 🔥 YENİ: Teknisyen depomuzu aldık!
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IValidator<CreateWorkOrderCommand> _validator;

    public CreateWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Customer> customerRepository,
        IGenericRepository<Technician> technicianRepository,
        IMapper mapper,
        INotificationService notificationService,
        IValidator<CreateWorkOrderCommand> validator)
    {
        _workOrderRepository = workOrderRepository;
        _customerRepository = customerRepository;
        _technicianRepository = technicianRepository;
        _mapper = mapper;
        _notificationService = notificationService;
        _validator = validator;
    }

    public async Task<int> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Validation Kontrolü
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 2. Müşteri Kontrolü
        await _customerRepository.GetActiveByIdOrThrowAsync(request.CustomerId, "müşteri");

        // 3. 🔥 YENİ: Teknisyen Kontrolü (Eğer bir teknisyen seçildiyse)
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

        // 4. Mapping ve Entity Hazırlığı
        var entity = _mapper.Map<WorkOrder>(request);
        entity.CreatedAt = DateTime.UtcNow;

        // 5. Veritabanına Ekle ve KAYDET (Fırını Çalıştır!) 🔥
        await _workOrderRepository.AddAsync(entity);
        await _workOrderRepository.SaveChangesAsync(); // <-- İşte hayat kurtaran satır burası!

        // 6. SignalR / Bildirim Ateşlemesi
        var customer = await _customerRepository.GetActiveByIdOrThrowAsync(request.CustomerId, "müşteri");
        await _notificationService.SendWorkOrderNotification($"#{entity.Id} no'lu yeni iş emri açıldı! Müşteri: {customer.FirstName} {customer.LastName} | Atanan: {technicianName}");

        return entity.Id;
    }
}