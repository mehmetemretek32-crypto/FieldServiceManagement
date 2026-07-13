using FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;
using FeaturesCreateValidator = FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder.CreateWorkOrderCommandValidator;
using SharedCreateValidator = FSM.Application.Validators.CreateWorkOrderCommandValidator;
using FSM.Application.Validators;

namespace FSM.Tests.Validators;

public class AssignWorkOrderValidatorTests
{
    private readonly AssignWorkOrderCommandValidator _validator = new();

    [Fact]
    public void Passes_For_ValidCommand()
    {
        var result = _validator.Validate(new AssignWorkOrderCommand { WorkOrderId = 1, TechnicianId = 2 });

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, -1)]
    public void Fails_When_Ids_NotPositive(int workOrderId, int technicianId)
    {
        var result = _validator.Validate(new AssignWorkOrderCommand
        {
            WorkOrderId = workOrderId,
            TechnicianId = technicianId
        });

        Assert.False(result.IsValid);
    }
}

public class SharedCreateWorkOrderValidatorTests
{
    private readonly SharedCreateValidator _validator = new();

    private static CreateWorkOrderCommand ValidCommand() => new()
    {
        Title = "Fix boiler",
        Description = "Boiler needs maintenance",
        CustomerId = 5
    };

    [Fact]
    public void Passes_For_ValidCommand()
    {
        Assert.True(_validator.Validate(ValidCommand()).IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Fails_When_Title_TooShort(string title)
    {
        var command = ValidCommand();
        command.Title = title;

        Assert.False(_validator.Validate(command).IsValid);
    }

    [Fact]
    public void Fails_When_CustomerId_NotPositive()
    {
        var command = ValidCommand();
        command.CustomerId = 0;

        Assert.False(_validator.Validate(command).IsValid);
    }
}

public class FeaturesCreateWorkOrderValidatorTests
{
    private readonly FeaturesCreateValidator _validator = new();

    private static CreateWorkOrderCommand ValidCommand() => new()
    {
        Title = "Fix boiler",
        Description = "Boiler needs a full maintenance cycle",
        CustomerId = 5
    };

    [Fact]
    public void Passes_For_ValidCommand()
    {
        Assert.True(_validator.Validate(ValidCommand()).IsValid);
    }

    [Fact]
    public void Fails_When_Description_TooShort()
    {
        var command = ValidCommand();
        command.Description = "short";

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Description));
    }
}

public class UpdateWorkOrderValidatorTests
{
    private readonly UpdateWorkOrderCommandValidator _validator = new();

    private static UpdateWorkOrderCommand ValidCommand() => new()
    {
        Id = 1,
        Title = "Fix boiler",
        Description = "Boiler needs maintenance"
    };

    [Fact]
    public void Passes_For_ValidCommand()
    {
        Assert.True(_validator.Validate(ValidCommand()).IsValid);
    }

    [Fact]
    public void Fails_When_Id_NotPositive()
    {
        var command = ValidCommand();
        command.Id = 0;

        Assert.False(_validator.Validate(command).IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Fails_When_Title_TooShort(string title)
    {
        var command = ValidCommand();
        command.Title = title;

        Assert.False(_validator.Validate(command).IsValid);
    }

    [Fact]
    public void Fails_When_Description_TooShort()
    {
        var command = ValidCommand();
        command.Description = "abc";

        Assert.False(_validator.Validate(command).IsValid);
    }
}
