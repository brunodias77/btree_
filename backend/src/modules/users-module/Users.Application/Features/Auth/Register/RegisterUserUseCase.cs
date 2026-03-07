using Shared.Application.Data;
using Shared.Application.Models;
using Users.Application.Services;
using Users.Domain.Aggregates.Profiles;
using Users.Domain.Errors;
using Users.Domain.Repositories;

namespace Users.Application.Features.Auth.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    
    private readonly IIdentityService _identityService;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<RegisterUserInput> _validator;
    
    public RegisterUserUseCase(
        IIdentityService identityService, 
        IProfileRepository profileRepository, 
        IUnitOfWork unitOfWork,
        FluentValidation.IValidator<RegisterUserInput> validator)
    {
        _identityService = identityService;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(RegisterUserInput input, CancellationToken cancellationToken = default)
    {
        // 0. Validar Input
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 1. Verificar se o email já está em uso
        var isEmailUnique = await _identityService.IsEmailUniqueAsync(input.Email, cancellationToken);
        
        if (!isEmailUnique)
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }
        
        // 2. Criar usuário no Identity (Auth)
        var identityResult = await _identityService.CreateUserAsync(
            input.Email, 
            input.Password, 
            cancellationToken);
        
        if (identityResult.IsFailure)
        {
            return Result.Failure<Guid>(identityResult.Error);
        }
        
        var userId = identityResult.Value;
        
        try
        {
            // 3. Criar agregado de Perfil (Domínio)
            // O método Profile.Create já dispara o ProfileCreatedDomainEvent
            var profile = Profile.Create(
                userId, 
                input.Email, 
                $"{input.FirstName} {input.LastName}", 
                input.FirstName, 
                input.LastName
            );
            
            
            // 4. Se houver CPF ou Data de Nascimento, atualiza o perfil
            if (!string.IsNullOrEmpty(input.Cpf) || input.BirthDate.HasValue)
            {
                profile.Update(
                    firstName: null, // Mantém o atual
                    lastName: null,
                    displayName: null, 
                    birthDate: input.BirthDate, 
                    gender: null, 
                    cpf: input.Cpf, 
                    preferredLanguage: null, 
                    preferredCurrency: null, 
                    newsletterSubscribed: null
                );
            }
            
            // 5. Persistir no repositório
            await _profileRepository.AddAsync(profile, cancellationToken);

            // 6. Salvar alterações (os eventos de domínio serão interceptados pelo DomainEventInterceptor)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(userId);

        }
        catch (Exception)
        {
            // Rollback: se falhar ao criar o perfil, deletar o usuário do Identity
            // para manter consistência entre os dois sistemas
            await _identityService.DeleteUserAsync(userId, cancellationToken);
            
            return Result.Failure<Guid>(UserErrors.RegistrationFailed);
        }


        
    }
}