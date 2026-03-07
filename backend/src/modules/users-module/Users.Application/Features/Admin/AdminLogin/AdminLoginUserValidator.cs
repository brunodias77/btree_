using FluentValidation;

namespace Users.Application.Features.Admin.AdminLogin;

public class AdminLoginUserValidator : AbstractValidator<AdminLoginUserInput>
{
    public AdminLoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("O email deve ser válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("O endereço IP é obrigatório.")
            .Matches(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}$|^([0-9a-fA-F]{1,4}:){1,7}:|^([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}$|^([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}$|^([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}$|^([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}$|^([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}$|^[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})$|:((:[0-9a-fA-F]{1,4}){1,7}|:)$")
            .WithMessage("O endereço IP deve ser válido (IPv4 ou IPv6).");

        RuleFor(x => x.DeviceName)
            .NotEmpty().WithMessage("O nome do dispositivo é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do dispositivo deve ter no máximo 100 caracteres.");

        RuleFor(x => x.DeviceType)
            .IsInEnum().WithMessage("Tipo de dispositivo inválido.");
    }
}