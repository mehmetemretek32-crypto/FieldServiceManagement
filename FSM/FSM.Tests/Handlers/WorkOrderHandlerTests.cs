using AutoMapper;
using FluentValidation;
using FSM.Application.Common;
using FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.DeleteWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrderStatus;
using FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders;
using FSM.Application.Features.WorkOrders.Queries.GetMyActiveWorkOrders;
using FSM.Application.Features.WorkOrders.Queries.GetWorkOrderById;
using FSM.Application.Interfaces;
using FSM.Application.Interfaces; // 🔥 Bildirim servisi için eklendi
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using FSM.Tests.TestUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Distributed; // 🔥 Redis önbellek için eklendi
using Microsoft.Extensions.Caching.Distributed; // 🔥 Redis Mock işlemleri için eklendi
using Moq;
using SharedCreateValidator = FSM.Application.Validators.CreateWorkOrderCommandValidator;
using Microsoft.AspNetCore.Http;
namespace FSM.Tests.Handlers;

public class AssignWorkOrderCommandHandlerTests
{
    private readonly Mock<IGenericRepository<WorkOrder>> _workOrders = new();
    private readonly Mock<IGenericRepository<Technician>> _technicians = new();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Eklendi

    // Handler'ı 3 parametre ile başlatıyoruz
    private AssignWorkOrderCommandHandler CreateHandler() =>
        new(_workOrders.Object, _technicians.Object, _mockCache.Object); // 🔥 Parametre eklendi

    [Fact]
    public async Task Handle_AssignsTechnician_AndSaves()
    {
        var workOrder = new WorkOrder { Id = 1, Title = "Fix boiler", State = WorkOrderState.Pending };
        var technician = new Technician { Id = 5, FullName = "Jane", IsDeleted = false };

        _workOrders.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(workOrder);
        _technicians.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(technician);

        // YENİ: C# Record yapımıza uygun olarak 4 parametreyi parantez içinde gönderiyoruz! (Tarihleri uyduruyoruz)
        var command = new AssignWorkOrderCommand(1, 5, DateTime.UtcNow, DateTime.UtcNow.AddHours(2));
        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(5, workOrder.TechnicianId);
        Assert.Equal(WorkOrderState.Assigned, workOrder.State); // Sende Assigned yoksa burası kızarabilir, uygun olanı yazarsın.
       
        _workOrders.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenWorkOrderMissing()
    {
        _workOrders.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((WorkOrder?)null);
        _technicians.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Technician { Id = 5 });

        // YENİ: Yine 4 parametreli yeni yapıyı kullanıyoruz
        var command = new AssignWorkOrderCommand(1, 5, DateTime.UtcNow, DateTime.UtcNow.AddHours(2));
        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(command, CancellationToken.None));
    }
}

public class CreateWorkOrderCommandHandlerTests
{
    private readonly Mock<IGenericRepository<WorkOrder>> _workOrders = new();
    private readonly Mock<IGenericRepository<Customer>> _customers = new();
    private readonly Mock<IGenericRepository<Technician>> _technicians = new();
    private readonly Mock<INotificationService> _notifications = new();
    private readonly IMapper _mapper = MapperFactory.Create();
    private readonly IValidator<CreateWorkOrderCommand> _validator = new SharedCreateValidator();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Eklendi

    private CreateWorkOrderCommandHandler CreateHandler() =>
        new(_workOrders.Object, _customers.Object, _technicians.Object, _mapper, _notifications.Object, _validator, _mockCache.Object); // 🔥 Parametre eklendi

    private static CreateWorkOrderCommand ValidCommand() => new()
    {
        Title = "Fix boiler",
        Description = "Boiler needs maintenance",
        CustomerId = 5
    };

    [Fact]
    public async Task Handle_CreatesWorkOrder_AndNotifies()
    {
        _customers.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Customer { Id = 5, IsDeleted = false });
        _workOrders.Setup(r => r.AddAsync(It.IsAny<WorkOrder>()))
            .Callback<WorkOrder>(w => w.Id = 77)
            .Returns(Task.CompletedTask);

        var id = await CreateHandler().Handle(ValidCommand(), CancellationToken.None);

        Assert.Equal(77, id);
        _workOrders.Verify(r => r.AddAsync(It.Is<WorkOrder>(w => w.Title == "Fix boiler")), Times.Once);
        _notifications.Verify(n => n.SendWorkOrderNotification(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenCommandInvalid()
    {
        var command = ValidCommand();
        command.Title = "";
        command.CustomerId = 0;

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().Handle(command, CancellationToken.None));

        _workOrders.Verify(r => r.AddAsync(It.IsAny<WorkOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_WhenCustomerMissing()
    {
        _customers.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(ValidCommand(), CancellationToken.None));

        _workOrders.Verify(r => r.AddAsync(It.IsAny<WorkOrder>()), Times.Never);
    }
}

public class UpdateWorkOrderCommandHandlerTests
{
    private readonly Mock<IGenericRepository<WorkOrder>> _repository = new();
    private readonly Mock<IGenericRepository<Technician>> _technicianRepository = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Eklendi

    // 🔥 Eksik parametreler Constructor'a gönderildi
    private UpdateWorkOrderCommandHandler CreateHandler() => new(_repository.Object, _technicianRepository.Object, _mockNotificationService.Object, _mockCache.Object);

    [Fact]
    public async Task Handle_UpdatesTitleAndDescription()
    {
        var workOrder = new WorkOrder { Id = 1, Title = "Old", Description = "Old desc" };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(workOrder);

        await CreateHandler().Handle(new UpdateWorkOrderCommand
        {
            Id = 1,
            Title = "New title",
            Description = "New description"
        }, CancellationToken.None);

        Assert.Equal("New title", workOrder.Title);
        Assert.Equal("New description", workOrder.Description);
        
    }

    [Fact]
    public async Task Handle_Throws_WhenWorkOrderMissing()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((WorkOrder?)null);

        // Not: NotFoundException sınıfının projedeki Namespace'i usings kısmında ekli olmalıdır.
        await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(
            new UpdateWorkOrderCommand { Id = 1, Title = "New", Description = "New description" }, CancellationToken.None));
    }
}

public class DeleteWorkOrderCommandHandlerTests
{
    private readonly Mock<IGenericRepository<WorkOrder>> _repository = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Eklendi

    // 🔥 Eksik parametre eklendi
    private DeleteWorkOrderCommandHandler CreateHandler() => new(_repository.Object, _mockNotificationService.Object, _mockCache.Object);

    [Fact]
    public async Task Handle_SoftDeletesWorkOrder()
    {
        var workOrder = new WorkOrder { Id = 1, IsDeleted = false };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(workOrder);

        await CreateHandler().Handle(new DeleteWorkOrderCommand { Id = 1 }, CancellationToken.None);

        Assert.True(workOrder.IsDeleted);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenAlreadyDeleted()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new WorkOrder { Id = 1, IsDeleted = true });

        await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(
            new DeleteWorkOrderCommand { Id = 1 }, CancellationToken.None));
    }
}

public class UpdateWorkOrderStatusCommandHandlerTests
{
    private readonly Mock<IGenericRepository<WorkOrder>> _repository = new();
    private readonly Mock<INotificationService> _notifications = new();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Eklendi

    // 🔥 Eksik parametre eklendi
    private UpdateWorkOrderStatusCommandHandler CreateHandler() =>
        new(_repository.Object, _notifications.Object, _mockCache.Object);

    [Theory]
    [InlineData("Completed", WorkOrderState.Completed)]
    [InlineData("inprogress", WorkOrderState.InProgress)]
    public async Task Handle_ParsesStatusCaseInsensitively(string input, WorkOrderState expected)
    {
        var workOrder = new WorkOrder { Id = 1, State = WorkOrderState.Pending, IsDeleted = false };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(workOrder);

        var result = await CreateHandler().Handle(
            new UpdateWorkOrderStatusCommand { WorkOrderId = 1, NewStatus = input }, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(expected, workOrder.State);
        _notifications.Verify(n => n.SendWorkOrderNotification(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenStatusInvalid()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new WorkOrder { Id = 1, IsDeleted = false });

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new UpdateWorkOrderStatusCommand { WorkOrderId = 1, NewStatus = "NotAState" }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenWorkOrderDeleted()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new WorkOrder { Id = 1, IsDeleted = true });

        await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(
            new UpdateWorkOrderStatusCommand { WorkOrderId = 1, NewStatus = "Completed" }, CancellationToken.None));
    }
}



public class GetMyActiveWorkOrdersQueryHandlerTests
{
    private readonly Mock<IGenericRepository<WorkOrder>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Eklendi
     // 🆕 RBAC testi için eklendi
    [Fact]
    public async Task Handle_ReturnsOnlyTechniciansNonDeletedOrders()
    {
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<WorkOrder>
        {
            new() { Id = 1, Title = "Mine", TechnicianId = 5, IsDeleted = false },
            new() { Id = 2, Title = "MineDeleted", TechnicianId = 5, IsDeleted = true },
            new() { Id = 3, Title = "Others", TechnicianId = 9, IsDeleted = false }
        });

        // 🔥 Parametre eklendi
        var handler = new GetMyActiveWorkOrdersQueryHandler(_repository.Object, _mapper, _mockCache.Object);

        var result = (await handler.Handle(
            new GetMyActiveWorkOrdersQuery { TechnicianId = 5 }, CancellationToken.None)).ToList();

        Assert.Single(result);
        Assert.Equal("Mine", result[0].Title);
    }
}


public class GetWorkOrderByIdQueryHandlerTests
{
    private readonly Mock<IGenericRepository<WorkOrder>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();

    private GetWorkOrderByIdQueryHandler CreateHandler() => new(_repository.Object, _mapper);

    [Fact]
    public async Task Handle_ReturnsDto_WhenFound()
    {
        _repository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new WorkOrder { Id = 1, Title = "Fix boiler", State = WorkOrderState.Pending });

        var result = await CreateHandler().Handle(new GetWorkOrderByIdQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Fix boiler", result!.Title);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((WorkOrder?)null);

        var result = await CreateHandler().Handle(new GetWorkOrderByIdQuery(1), CancellationToken.None);

        Assert.Null(result);
    }
}
