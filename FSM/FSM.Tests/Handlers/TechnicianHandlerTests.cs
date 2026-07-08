using AutoMapper;
using FSM.Application.Features.Technican.Queries.GetAllTechnician;
using FSM.Application.Features.Technicians.Commands.CreateTechnician;
using FSM.Application.Features.Technicians.Commands.DeleteTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;
using FSM.Application.Features.Technicians.Queries.GetTechnicianById;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Tests.TestUtilities;
using Moq;

namespace FSM.Tests.Handlers;

public class CreateTechnicianCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Technician>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();

    [Fact]
    public async Task Handle_AddsTechnicianAndReturnsId()
    {
        _repository
            .Setup(r => r.AddAsync(It.IsAny<Technician>()))
            .Callback<Technician>(t => t.Id = 42)
            .Returns(Task.CompletedTask);

        var handler = new CreateTechnicianCommandHandler(_repository.Object, _mapper);

        var id = await handler.Handle(new CreateTechnicianCommand
        {
            FullName = "Jane Doe",
            Email = "jane@example.com",
            PhoneNumber = "+905551112233"
        }, CancellationToken.None);

        Assert.Equal(42, id);
        _repository.Verify(r => r.AddAsync(It.Is<Technician>(t => t.FullName == "Jane Doe")), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}

public class UpdateTechnicianCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Technician>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();

    private UpdateTechnicianCommandHandler CreateHandler() => new(_repository.Object, _mapper);

    [Fact]
    public async Task Handle_UpdatesTechnicianFields()
    {
        var technician = new Technician { Id = 1, FullName = "Old", IsDeleted = false };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(technician);

        await CreateHandler().Handle(new UpdateTechnicianCommand
        {
            Id = 1,
            FullName = "New Name",
            Email = "new@example.com",
            PhoneNumber = "+905551112233"
        }, CancellationToken.None);

        Assert.Equal("New Name", technician.FullName);
        Assert.Equal("new@example.com", technician.Email);
        _repository.Verify(r => r.UpdateAsync(technician), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenTechnicianDeleted()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Technician { Id = 1, IsDeleted = true });

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new UpdateTechnicianCommand { Id = 1, FullName = "New" }, CancellationToken.None));

        _repository.Verify(r => r.UpdateAsync(It.IsAny<Technician>()), Times.Never);
    }
}

public class DeleteTechnicianCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Technician>> _repository = new();

    private DeleteTechnicianCommandHandler CreateHandler() => new(_repository.Object);

    [Fact]
    public async Task Handle_SoftDeletesTechnician()
    {
        var technician = new Technician { Id = 1, IsDeleted = false };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(technician);

        await CreateHandler().Handle(new DeleteTechnicianCommand { Id = 1 }, CancellationToken.None);

        Assert.True(technician.IsDeleted);
        _repository.Verify(r => r.UpdateAsync(technician), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenTechnicianMissing()
    {
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Technician?)null);

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new DeleteTechnicianCommand { Id = 1 }, CancellationToken.None));
    }
}

public class GetAllTechniciansQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Technician>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();

    [Fact]
    public async Task Handle_ReturnsOnlyActiveTechnicians()
    {
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Technician>
        {
            new() { Id = 1, FullName = "Active", IsDeleted = false },
            new() { Id = 2, FullName = "Deleted", IsDeleted = true }
        });

        var handler = new GetAllTechniciansQueryHandler(_repository.Object, _mapper);

        var result = await handler.Handle(new GetAllTechniciansQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Active", result[0].FullName);
    }
}

public class GetTechnicianByIdQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Technician>> _repository = new();
    private readonly IMapper _mapper = MapperFactory.Create();

    private GetTechnicianByIdQueryHandler CreateHandler() => new(_repository.Object, _mapper);

    [Fact]
    public async Task Handle_ReturnsDto_WhenActive()
    {
        _repository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Technician { Id = 1, FullName = "Jane", IsDeleted = false });

        var result = await CreateHandler().Handle(new GetTechnicianByIdQuery { Id = 1 }, CancellationToken.None);

        Assert.Equal(1, result.Id);
        Assert.Equal("Jane", result.FullName);
    }

    [Fact]
    public async Task Handle_Throws_WhenDeleted()
    {
        _repository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Technician { Id = 1, IsDeleted = true });

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(
            new GetTechnicianByIdQuery { Id = 1 }, CancellationToken.None));
    }
}
