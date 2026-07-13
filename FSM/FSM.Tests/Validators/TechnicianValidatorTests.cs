using FSM.Application.Features.Technican.Commands.CreateTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;
using FSM.Application.Validators;

namespace FSM.Tests.Validators;

public class CreateTechnicianValidatorTests
{
    private readonly CreateTechnicianCommandValidator _validator = new();

    // 1. HATA ÇÖZÜMÜ: Süslü parantez yerine parametreli Constructor kullanıyoruz.
    private static CreateTechnicianCommand ValidCommand() =>
        new("Jane Doe", "jane@example.com", "+905551112233");

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
        // 2. HATA ÇÖZÜMÜ: Record sonradan değiştirilemediği için hatalı ismi direkt yaratırken veriyoruz.
        var command = new CreateTechnicianCommand(name, "jane@example.com", "+905551112233");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FullName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Fails_When_Email_Invalid(string email)
    {
        // 3. HATA ÇÖZÜMÜ
        var command = new CreateTechnicianCommand("Jane Doe", email, "+905551112233");

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
        // 4. HATA ÇÖZÜMÜ
        var command = new CreateTechnicianCommand("Jane Doe", "jane@example.com", phone);

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