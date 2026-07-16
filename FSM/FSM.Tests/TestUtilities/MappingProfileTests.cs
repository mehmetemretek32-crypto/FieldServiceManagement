using FSM.Application.DTOs;
using FSM.Application.DTOs.Technicians;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Features.Customers.Commands.CreateCustomer;
using FSM.Application.Features.Technican.Commands.CreateTechnician;
using FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using FSM.Domain.Entities;
using FSM.Domain.Enums;

namespace FSM.Tests.TestUtilities;

public class MappingProfileTests
{
    [Fact]
    public void Maps_CreateTechnicianCommand_To_Technician()
    {
        var mapper = MapperFactory.Create();
        var command = new CreateTechnicianCommand("Jane Doe", "jane@example.com", "+905551112233");

        var technician = mapper.Map<Technician>(command);

        Assert.Equal(command.FullName, technician.FullName);
        Assert.Equal(command.Email, technician.Email);
        Assert.Equal(command.PhoneNumber, technician.PhoneNumber);
    }

    [Fact]
    public void Maps_Technician_To_TechnicianDto()
    {
        var mapper = MapperFactory.Create();
        var technician = new Technician
        {
            Id = 7,
            FullName = "John Smith",
            Email = "john@example.com",
            PhoneNumber = "+905551112233",
            IsAvailable = true
        };

        var dto = mapper.Map<TechnicianDto>(technician);

        Assert.Equal(technician.Id, dto.Id);
        Assert.Equal(technician.FullName, dto.FullName);
        Assert.Equal(technician.Email, dto.Email);
        Assert.Equal(technician.PhoneNumber, dto.PhoneNumber);
        Assert.True(dto.IsAvailable);
    }

    [Fact]
    public void Maps_CreateCustomerCommand_To_Customer()
    {
        var mapper = MapperFactory.Create();
        var command = new CreateCustomerCommand
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            PhoneNumber = "+905551112233",
            CompanyName = "Analytical Engines",
            Address = "London"
        };

        var customer = mapper.Map<Customer>(command);

        Assert.Equal(command.FirstName, customer.FirstName);
        Assert.Equal(command.LastName, customer.LastName);
        Assert.Equal(command.Email, customer.Email);
        Assert.Equal(command.PhoneNumber, customer.PhoneNumber);
        Assert.Equal(command.CompanyName, customer.CompanyName);
        Assert.Equal(command.Address, customer.Address);
    }

    [Fact]
    public void Maps_Customer_To_CustomerDto()
    {
        var mapper = MapperFactory.Create();
        var customer = new Customer
        {
            Id = 3,
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            PhoneNumber = "+905551112233",
            CompanyName = "Analytical Engines",
            Address = "London"
        };

        var dto = mapper.Map<CustomerDto>(customer);

        Assert.Equal(customer.Id, dto.Id);
        Assert.Equal(customer.FirstName, dto.FirstName);
        Assert.Equal(customer.LastName, dto.LastName);
        Assert.Equal(customer.Address, dto.Address);
    }

    [Fact]
    public void Maps_CreateWorkOrderCommand_To_WorkOrder()
    {
        var mapper = MapperFactory.Create();
        var command = new CreateWorkOrderCommand
        {
            Title = "Fix boiler",
            Description = "Boiler needs maintenance",
            CustomerId = 42
        };

        var workOrder = mapper.Map<WorkOrder>(command);

        Assert.Equal(command.Title, workOrder.Title);
        Assert.Equal(command.Description, workOrder.Description);
        Assert.Equal(command.CustomerId, workOrder.CustomerId);
    }

    [Fact]
    public void Maps_WorkOrder_To_WorkOrderDto_SerializesStateAsName()
    {
        var mapper = MapperFactory.Create();
        var workOrder = new WorkOrder
        {
            Id = 11,
            Title = "Fix boiler",
            Description = "Boiler needs maintenance",
            State = WorkOrderState.InProgress,
            CustomerId = 42
        };

        var dto = mapper.Map<WorkOrderDto>(workOrder);

        Assert.Equal(workOrder.Id, dto.Id);
        // 131. satırı şu şekilde güncelle:
        // Enum değerini int'e cast ediyoruz ki int olan dto.State ile kıyaslayabilelim
        Assert.Equal((int)WorkOrderState.InProgress, dto.State);
        Assert.Equal(workOrder.CustomerId, dto.CustomerId);
    }
}
