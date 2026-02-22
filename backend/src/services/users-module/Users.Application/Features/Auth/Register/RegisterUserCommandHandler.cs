using BuildingBlocks.Application.Data;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Models;
using Users.Application.Repositories;
using Users.Application.Services;
using Users.Domain.Aggregates.Profile;
using Users.Domain.Errors;

namespace Users.Application.Features.Auth.Register;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid> 
{
    public RegisterUserCommandHandler(IIdentityService identityService, IProfileRepository profileRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    private readonly IIdentityService _identityService;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificar se o email já está em uso
        var isEmailUnique = await _identityService.IsEmailUniqueAsync(request.Email, cancellationToken);
        
        if (!isEmailUnique)
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }
        
        // 2. Criar usuário no Identity (Auth)
        var identityResult = await _identityService.CreateUserAsync(
            request.Email, 
            request.Password, 
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
                request.Email, 
                $"{request.FirstName} {request.LastName}", 
                request.FirstName, 
                request.LastName
            );
            
            
            // 4. Se houver CPF ou Data de Nascimento, atualiza o perfil
            if (!string.IsNullOrEmpty(request.Cpf) || request.BirthDate.HasValue)
            {
                profile.Update(
                    firstName: null, // Mantém o atual
                    lastName: null,
                    displayName: null, 
                    birthDate: request.BirthDate, 
                    gender: null, 
                    cpf: request.Cpf, 
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