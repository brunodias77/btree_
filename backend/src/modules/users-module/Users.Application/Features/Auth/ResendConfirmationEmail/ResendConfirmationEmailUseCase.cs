using Microsoft.Extensions.Configuration;
using Shared.Application.Abstractions;
using Shared.Application.Models;
using System.Text;
using Users.Application.Services;
using Users.Domain.Errors;

namespace Users.Application.Features.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailUseCase : IResendConfirmationEmailUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly FluentValidation.IValidator<ResendConfirmationEmailInput> _validator;

    public ResendConfirmationEmailUseCase(
        IIdentityService identityService,
        IEmailService emailService,
        IConfiguration configuration,
        FluentValidation.IValidator<ResendConfirmationEmailInput> validator)
    {
        _identityService = identityService;
        _emailService = emailService;
        _configuration = configuration;
        _validator = validator;
    }

    public async Task<Result<Result>> ExecuteAsync(ResendConfirmationEmailInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Result>(new Error("Validation.Error", validationResult.ToString()));
        }

        var user = await _identityService.GetByEmailAsync(input.Email, cancellationToken);
        
        if (user is null)
        {
            // Retorna Silent Success para evitar Email Enumeration
            return Result.Success(Result.Success());
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure<Result>(AuthErrors.EmailAlreadyConfirmed);
        }

        var tokenResult = await _identityService.GenerateEmailConfirmationTokenAsync(user.Id, cancellationToken);
        if (tokenResult.IsFailure)
        {
            return Result.Failure<Result>(tokenResult.Error);
        }

        var tokenEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenResult.Value));
        var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:4200";
        var confirmationLink = $"{frontendUrl}/auth/confirm-email?email={user.Email}&token={tokenEncoded}";

        var subject = "Btree - Confirmação de E-mail";
        var body = $"<html><body><p>Você solicitou um novo e-mail de confirmação.</p><p>Clique no link abaixo para confirmar sua conta:</p><p><a href='{confirmationLink}'>Confirmar E-mail</a></p></body></html>";

        await _emailService.SendEmailAsync(user.Email!, subject, body);

        return Result.Success(Result.Success());
    }
}
