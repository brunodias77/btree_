using Shared.Application.Abstractions;
using Shared.Application.Data;
using Shared.Application.Models;
using Users.Application.Services;
using Users.Domain.Repositories;
using Users.Domain.Aggregates.Profiles.Events;

namespace Users.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordUseCase : IForgotPasswordUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly FluentValidation.IValidator<ForgotPasswordInput> _validator;

    public ForgotPasswordUseCase(
        IIdentityService identityService,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        FluentValidation.IValidator<ForgotPasswordInput> validator)
    {
        _identityService = identityService;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _validator = validator;
    }

    public async Task<Result<Result>> ExecuteAsync(ForgotPasswordInput input, CancellationToken cancellationToken = default)
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

        // Buscar perfil para adicionar evento de domínio
        var profile = await _profileRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (profile is null)
        {
            // Retorna Silent Success (ou falha técnica se aplicável ao projeto, mantido Silent Success por segurança)
            return Result.Success(Result.Success());
        }

        var tokenResult = await _identityService.GeneratePasswordResetTokenAsync(user.Email!, cancellationToken);
        if (tokenResult.IsFailure)
        {
            return Result.Failure<Result>(tokenResult.Error);
        }

        var resetCode = new Random().Next(100000, 999999).ToString();

        // Como ApplicationUser nao suporta eventos no EF, acoplamos ao Profile.
        profile.RequestPasswordReset(user.Email!, tokenResult.Value, resetCode);

        _profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send Email
        var resetLink = $"http://localhost:4200/reset-password?code={resetCode}"; // Adjust domain depending on configuration if applicable
        var subject = "Redefinição de Senha";
        var body = $@"
            <h1>Redefinição de Senha</h1>
            <p>Você solicitou a redefinição de sua senha.</p>
            <p>Seu código de verificação é: <strong>{resetCode}</strong></p>
            <p>Você pode redefinir sua senha clicando no link abaixo ou inserindo o código no aplicativo:</p>
            <p><a href='{resetLink}'>Redefinir Senha</a></p>
            <p>Se você não solicitou a redefinição de senha, ignore este e-mail.</p>
        ";
        await _emailService.SendEmailAsync(user.Email!, subject, body);

        return Result.Success(Result.Success());
    }
}
