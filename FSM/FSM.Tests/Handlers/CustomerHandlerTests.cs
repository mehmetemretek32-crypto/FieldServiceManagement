using AutoMapper;
using FSM.Application.Features.Customers.Commands.CreateCustomer;
using FSM.Application.Features.Customers.Commands.DeleteCustomer;
using FSM.Application.Features.Customers.Commands.UpdateCustomer;
using FSM.Application.Features.Customers.Queries.GetAllCustomers;
using FSM.Application.Features.Customers.Queries.GetCustomerById;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces; // 🔥 Bildirim servisi arayüzü için
using Microsoft.Extensions.Caching.Distributed; // 🔥 Redis arayüzü için
using FSM.Tests.TestUtilities;
using Moq;
using Microsoft.Extensions.Caching.Distributed;

namespace FSM.Tests.Handlers;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Customer>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();
    private readonly Mock<INotificationService> _mockNotificationService = new();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Cache Mock Eklendi

    [Fact]
    public async Task Handle_AddsCustomerAndReturnsId()
    {
        _repository
            .Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .Callback<Customer>(c => c.Id = 99)
            .Returns(Task.CompletedTask);

        // 🔥 Parametre olarak eklendi
        var handler = new CreateCustomerCommandHandler(_repository.Object, _mapper, _mockNotificationService.Object, _mockCache.Object);

        var id = await handler.Handle(new CreateCustomerCommand
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            PhoneNumber = "+905551112233",
            CompanyName = "Analytical Engines",
            Address = "London"
        }, CancellationToken.None);

        Assert.Equal(99, id);
        _repository.Verify(r => r.AddAsync(It.Is<Customer>(c => c.FirstName == "Ada")), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}

public class UpdateCustomerCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Customer>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();
    private readonly Mock<INotificationService> _mockNotificationService = new();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Cache Mock Eklendi

    // 🔥 Parametre olarak eklendi
    private UpdateCustomerCommandHandler CreateHandler() => new(_repository.Object, _mapper, _mockNotificationService.Object, _mockCache.Object);

    [Fact]
    public async Task Handle_UpdatesExistingCustomerFields()
    {
        var customer = new Customer { Id = 1, FirstName = "Old", IsDeleted = false };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        await CreateHandler().Handle(new UpdateCustomerCommand
        {
            Id = 1,
            FirstName = "New",
            LastName = "Name",
            Email = "new@example.com",
            PhoneNumber = "+905551112233",
            CompanyName = "Corp",
            Address = "Somewhere"
        }, CancellationToken.None);

        Assert.Equal("New", customer.FirstName);
        Assert.Equal("Name", customer.LastName);
        Assert.Equal("new@example.com", customer.Email);
        Assert.Equal("+905551112233", customer.PhoneNumber);
        _repository.Verify(r => r.UpdateAsync(customer), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenCustomerMissingOrDeleted()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Customer { Id = 1, IsDeleted = true });

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new UpdateCustomerCommand { Id = 1, FirstName = "New", LastName = "Name" },
            CancellationToken.None));

        _repository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
    }
}

public class DeleteCustomerCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Customer>> _repository = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Cache Mock Eklendi

    // 🔥 Parametre olarak eklendi
    private DeleteCustomerCommandHandler CreateHandler() => new(_repository.Object, _mockNotificationService.Object, _mockCache.Object);

    [Fact]
    public async Task Handle_SoftDeletesCustomer()
    {
        var customer = new Customer { Id = 1, IsDeleted = false };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        await CreateHandler().Handle(new DeleteCustomerCommand { Id = 1 }, CancellationToken.None);

        Assert.True(customer.IsDeleted);
        _repository.Verify(r => r.UpdateAsync(customer), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenCustomerNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new DeleteCustomerCommand { Id = 1 }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenCustomerAlreadyDeleted()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Customer { Id = 1, IsDeleted = true });

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new DeleteCustomerCommand { Id = 1 }, CancellationToken.None));

        _repository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
    }
}

public class GetAllCustomersQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Customer>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();
    private readonly Mock<IGenericRepository<WorkOrder>> _workOrderRepository = new();
    private readonly Mock<IDistributedCache> _mockCache = new(); // 🔥 Cache Mock Eklendi

    [Fact]
    public async Task Handle_ReturnsOnlyActiveCustomers()
    {
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Customer>
        {
            new() { Id = 1, FirstName = "Active", IsDeleted = false },
            new() { Id = 2, FirstName = "Deleted", IsDeleted = true }
        });

        // Cache'in boş dönmesini sağlayarak (simüle ederek) doğrudan SQL tarafındaki filtrelere gitmesini test ediyoruz.
        var handler = new GetAllCustomersQueryHandler(
            _repository.Object,
            _workOrderRepository.Object,
            _mapper,
            _mockCache.Object); // 🔥 Parametre eklendi

        var result = (await handler.Handle(new GetAllCustomersQuery(), CancellationToken.None)).ToList();

        Assert.Single(result);
        Assert.Equal("Active", result[0].FirstName);
    }
}

public class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Customer>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();

    // Bu sınıfta bir değişiklik yapmadıysan orijinal haliyle kalabilir.
    private GetCustomerByIdQueryHandler CreateHandler() => new(_repository.Object, _mapper);

    [Fact]
    public async Task Handle_ReturnsDto_WhenCustomerActive()
    {
        _repository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Customer { Id = 1, FirstName = "Ada", IsDeleted = false });

        var result = await CreateHandler().Handle(new GetCustomerByIdQuery { Id = 1 }, CancellationToken.None);

        Assert.Equal(1, result.Id);
        Assert.Equal("Ada", result.FirstName);
    }

    [Fact]
    public async Task Handle_Throws_WhenCustomerDeleted()
    {
        _repository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Customer { Id = 1, IsDeleted = true });

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new GetCustomerByIdQuery { Id = 1 }, CancellationToken.None));
    }
}