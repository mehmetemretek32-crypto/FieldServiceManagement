using AutoMapper;
using FluentValidation;
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
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IValidator<CreateWorkOrderCommand> _validator;

    public CreateWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Customer> customerRepository,
        IMapper mapper,
        INotificationService notificationService,
        IValidator<CreateWorkOrderCommand> validator)
    {
        _workOrderRepository = workOrderRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
        _notificationService = notificationService;
        _validator = validator;
    }

    public async Task<int> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Validation
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 2. Müşteri Kontrolü
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null || customer.IsDeleted)
        {
            throw new Exception($"Hata: ID'si {request.CustomerId} olan aktif bir müşteri bulunamadı!");
        }

        // 3. Mapping ve Entity Hazırlığı
        var entity = _mapper.Map<WorkOrder>(request);
        entity.CreatedAt = DateTime.UtcNow;

        // Status Enum ise Enum.Parse kullanılır, string ise direkt eşitle
        // entity.Status = Enum.Parse<WorkOrderStatus>(request.Status); 

        // 4. Kayıt
        await _workOrderRepository.AddAsync(entity);

        // 5. Bildirim
        await _notificationService.SendWorkOrderNotification("Yeni iş emri atandı: " + entity.Id);

        return entity.Id;
    }
}