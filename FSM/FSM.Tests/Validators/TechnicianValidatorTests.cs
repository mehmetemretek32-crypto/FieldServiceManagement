using FSM.Application.Features.Technicians.Commands.CreateTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;
using FSM.Application.Validators;

namespace FSM.Tests.Validators;

public class CreateTechnicianValidatorTests
{
    private readonly CreateTechnicianCommandValidator _validator = new();

    private static CreateTechnicianCommand ValidCommand() => new()
    {
        FullName = "Jane Doe",
        Email = "jane@example.com",
        PhoneNumber = "+905551112233"
    };

    [Fact]
    public void Passes_For_ValidCommand()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("J")]
    public void Fails_When_FullName_TooShortOrEmpty(string name)
    {
        var command = ValidCommand();
        command.FullName = name;

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FullName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Fails_When_Email_Invalid(string email)
    {
        var command = ValidCommand();
        command.Email = email;

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Email));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("phone")]
    [InlineData("")]
    public void Fails_When_PhoneNumber_Invalid(string phone)
    {
        var command = ValidCommand();
        command.PhoneNumber = phone;

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.PhoneNumber));
    }
}

public class UpdateTechnicianValidatorTests
{
    private readonly UpdateTechnicianCommandValidator _validator = new();

    private static UpdateTechnicianCommand ValidCommand() => new()
    {
        Id = 1,
        FullName = "Jane Doe",
        Email = "jane@example.com",
        PhoneNumber = "+905551112233"
    };

    [Fact]
    public void Passes_For_ValidCommand()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Fails_When_Id_NotPositive()
    {
        var command = ValidCommand();
        command.Id = 0;

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Id));
    }

    [Fact]
    public void Fails_When_FullName_TooShort()
    {
        var command = ValidCommand();
        command.FullName = "Jo";

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FullName));
    }
}
