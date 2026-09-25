using FluentValidation;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Features.Auth.Commands;
using NunyFoodWebApi.Application.Features.Customers.Commands;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;
using NunyFoodWebApi.Tests.Support;

namespace NunyFoodWebApi.Tests;

public class AuthTests
{
    private readonly TestApp _app = new();
    private readonly Customer _customer;

    public AuthTests() => _customer = _app.AddCustomer("ama@test.local");

    private Task<Application.DTOs.Auth.OtpChallengeDto?> Login(string password = TestApp.Password) =>
        _app.Send(new LoginCommand(_customer.Email, password, Roles.Customer));

    private Task<Application.DTOs.Auth.TokenDto?> Verify(string code) =>
        _app.Send(new VerifyOtpCommand(_customer.Email, code, Roles.Customer));

    [Fact]
    public async Task Login_with_wrong_password_sends_no_code()
    {
        Assert.Null(await Login("Mauvais@123"));
        Assert.Empty(_app.Emails.Sent);
    }

    [Fact]
    public async Task Login_then_emailed_code_gives_a_token_once()
    {
        Assert.NotNull(await Login());
        var code = _app.Emails.LastCodeFor(_customer.Email, OtpPurpose.Login);

        Assert.NotNull(await Verify(code));
        Assert.Null(await Verify(code)); // code à usage unique
    }

    [Fact]
    public async Task Code_is_destroyed_after_five_wrong_attempts()
    {
        await Login();
        var code = _app.Emails.LastCodeFor(_customer.Email, OtpPurpose.Login);
        var wrong = code == "000000" ? "111111" : "000000";

        for (var i = 0; i < 5; i++)
            Assert.Null(await Verify(wrong));

        Assert.Null(await Verify(code));
    }

    [Fact]
    public async Task Expired_code_is_refused()
    {
        await Login();
        var code = _app.Emails.LastCodeFor(_customer.Email, OtpPurpose.Login);
        _app.Set<OtpCode>().Single().ExpiresAt = DateTime.UtcNow.AddSeconds(-1);

        Assert.Null(await Verify(code));
    }

    [Fact]
    public async Task Forgot_password_gives_the_same_answer_for_unknown_emails_without_sending_anything()
    {
        var challenge = await _app.Send(new ForgotPasswordCommand("inconnu@test.local"));

        Assert.Equal("inconnu@test.local", challenge.Email);
        Assert.Empty(_app.Emails.Sent);
    }

    [Fact]
    public async Task Reset_password_with_emailed_code_changes_the_password()
    {
        await _app.Send(new ForgotPasswordCommand(_customer.Email));
        var code = _app.Emails.LastCodeFor(_customer.Email, OtpPurpose.PasswordReset);

        await _app.Send(new ResetPasswordCommand(_customer.Email, code, "Nouveau@12345"));

        Assert.True(_app.Hasher.Verify("Nouveau@12345", _customer.PasswordHash));
        Assert.Null(await Login());
    }

    [Fact]
    public async Task Login_code_cannot_be_used_to_reset_the_password()
    {
        await Login();
        var loginCode = _app.Emails.LastCodeFor(_customer.Email, OtpPurpose.Login);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _app.Send(new ResetPasswordCommand(_customer.Email, loginCode, "Nouveau@12345")));

        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(ResetPasswordCommand.Code));
        Assert.True(_app.Hasher.Verify(TestApp.Password, _customer.PasswordHash));
    }

    [Fact]
    public async Task Wrong_current_password_is_a_validation_error_not_a_401()
    {
        _app.SignInAs(_customer.Id, Roles.Customer);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _app.Send(new ChangePasswordCommand(_customer.Id, "Mauvais@123", "Nouveau@12345")));

        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.CurrentPassword));
    }

    [Fact]
    public async Task Customer_cannot_change_another_customers_password()
    {
        var other = _app.AddCustomer("autre@test.local");
        _app.SignInAs(_customer.Id, Roles.Customer);

        Assert.False(await _app.Send(new ChangePasswordCommand(other.Id, TestApp.Password, "Pirate@12345")));
        Assert.True(_app.Hasher.Verify(TestApp.Password, other.PasswordHash));
    }
}
