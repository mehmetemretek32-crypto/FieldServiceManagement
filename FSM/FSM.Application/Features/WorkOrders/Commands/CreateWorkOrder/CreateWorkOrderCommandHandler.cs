using AutoMapper;
using FluentValidation;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, int>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Customer> _customerRepository; // Müşteri kontrolü için eklendi!
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IValidator<CreateWorkOrderCommand> _validator; // Doğrulama için eklendi!

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
        // 1. VALIDATION KONTROLÜ (Başlık, açıklama boş mu?)
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 2. MÜŞTERİ KONTROLÜ (Olmayan veya silinmiş müşteriye atama engeli)
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null || customer.IsDeleted)
        {
            throw new Exception($"Hata: ID'si {request.CustomerId} olan aktif bir müşteri bulunamadı! İş emri oluşturulamaz.");
        }

        // 3. İŞLEM (Her şey yolundaysa kaydet)
        var entity = _mapper.Map<WorkOrder>(request);
        entity.CreatedAt = DateTime.UtcNow;
        // entity.State = WorkOrderState.Pending; // Eğer Enum kullanıyorsan burayı açabilirsin

        await _workOrderRepository.AddAsync(entity);
        // AddAsync içinde SaveChanges yoksa manuel olarak çağır: await _workOrderRepository.SaveChangesAsync();

        await _notificationService.SendWorkOrderNotification("Yeni iş emri atandı: " + entity.Id);

        return entity.Id;
    }
}