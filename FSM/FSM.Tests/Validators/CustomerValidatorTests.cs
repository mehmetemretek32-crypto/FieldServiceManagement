using FSM.Application.Features.Customers.Commands.CreateCustomer;
using FSM.Application.Features.Customers.Commands.UpdateCustomer;
using FSM.Application.Validators;

namespace FSM.Tests.Validators;

public class CreateCustomerValidatorTests
{
    private readonly CreateCustomerValidator _validator = new();

    private static CreateCustomerCommand ValidCommand() => new()
    {
        FirstName = "Ada",
        LastName = "Lovelace",
        CompanyName = "Analytical Engines",
        PhoneNumber = "+905551112233",
        Address = "London",
        Email = "ada@example.com"
    };

    [Fact]
    public void Passes_For_ValidCommand()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Fails_When_FirstName_TooShortOrEmpty(string firstName)
    {
        var command = ValidCommand();
        command.FirstName = firstName;

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FirstName));
    }

    [Fact]
    public void Fails_When_FirstName_ExceedsMaxLength()
    {
        var command = ValidCommand();
        command.FirstName = new string('a', 51);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FirstName));
    }

    [Fact]
    public void Fails_When_CompanyName_Empty()
    {
        var command = ValidCommand();
        command.CompanyName = "";

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.CompanyName));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("not-a-number")]
    [InlineData("")]
    public void Fails_When_PhoneNumber_Invalid(string phone)
    {
        var command = ValidCommand();
        command.PhoneNumber = phone;

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.PhoneNumber));
    }

    [Theory]
    [InlineData("0532 123 45 67")]
    [InlineData("invalid")]
    [InlineData("")]
    public void Fails_When_Email_Invalid(string email)
    {
        var command = ValidCommand();
        command.Email = email;

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Email));
    }
}

public class UpdateCustomerValidatorTests
{
    private readonly UpdateCustomerCommandValidator _validator = new();

    private static UpdateCustomerCommand ValidCommand() => new()
    {
        Id = 1,
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = "ada@example.com",
        PhoneNumber = "+905551112233",
        CompanyName = "Analytical Engines",
        Address = "London"
    };

    [Fact]
    public void Passes_For_ValidCommand()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Fails_When_Id_NotPositive(int id)
    {
        var command = ValidCommand();
        command.Id = id;

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Id));
    }

    [Fact]
    public void Fails_When_Email_Invalid()
    {
        var command = ValidCommand();
        command.Email = "nope";

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Email));
    }
}
