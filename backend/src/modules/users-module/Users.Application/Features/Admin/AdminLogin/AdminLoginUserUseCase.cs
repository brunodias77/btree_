using System.Security.Claims;
using Shared.Application.Data;
using Shared.Application.Models;
using Shared.Security.Abstractions;
using Users.Application.Services;
using Users.Domain.Aggregates.LoginHistory;
using Users.Domain.Aggregates.Sessions;
using Users.Domain.Errors;
using Users.Domain.Repositories;

namespace Users.Application.Features.Admin.AdminLogin;

public class AdminLoginUserUseCase : IAdminLoginUserUseCase
{


    private readonly IIdentityService _identityService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IProfileRepository _profileRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILoginHistoryRepository _loginHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<AdminLoginUserInput> _validator;

    private const int RefreshTokenExpirationDays = 7;
    private const int MaxDeviceNameLength = 100;
    private const string AdminRoleName = "Admin";
    
    public AdminLoginUserUseCase(IIdentityService identityService, ITokenGenerator tokenGenerator, IProfileRepository profileRepository, ISessionRepository sessionRepository, ILoginHistoryRepository loginHistoryRepository, IUnitOfWork unitOfWork,  FluentValidation.IValidator<AdminLoginUserInput> validator)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _profileRepository = profileRepository;
        _sessionRepository = sessionRepository;
        _loginHistoryRepository = loginHistoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;

    }

    private static string? TruncateDeviceName(string? deviceName)
    {
        if (string.IsNullOrEmpty(deviceName))
            return deviceName;

        return deviceName.Length > MaxDeviceNameLength
            ? deviceName[..MaxDeviceNameLength]
            : deviceName;
    }

    public async Task<Result<AdminLoginUserOutput>> ExecuteAsync(AdminLoginUserInput input, CancellationToken cancellationToken = default)
    {
        // 0. Validar Input
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<AdminLoginUserOutput>(new Error("Validation.Error", validationResult.ToString()));
        }
        
        
        // 1. Validar credenciais via Identity Service
        var credentialsResult = await _identityService.ValidateCredentialsAsync(
            input.Email,
            input.Password,
            cancellationToken);
        if (credentialsResult.IsFailure)
        {
            var existingUser = await _identityService.GetByEmailAsync(input.Email, cancellationToken);
            if (existingUser != null)
            {
                var failedDeviceInfo = !string.IsNullOrEmpty(input.DeviceName)
                    ? System.Text.Json.JsonSerializer.Serialize(new { userAgent = input.DeviceName })
                    : null;

                var failedHistory = LoginHistory.CreateFailure(
                    existingUser.Id,
                    "Credenciais inválidas",
                    loginProvider: "AdminPanel",
                    ipAddress: input.IpAddress,
                    deviceType: input.DeviceType.ToString(),
                    deviceInfo: failedDeviceInfo);

                await _loginHistoryRepository.AddAsync(failedHistory, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result.Failure<AdminLoginUserOutput>(AuthErrors.CredenciaisInvalidas);
        }
        
        
        var user = credentialsResult.Value;

        // 2. Verificar se a conta está ativa (email confirmado)
        if (!user.EmailConfirmed)
        {
            return Result.Failure<AdminLoginUserOutput>(AuthErrors.EmailNaoConfirmado);
        }

        // 3. Verificar se a conta está bloqueada
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
        {
            return Result.Failure<AdminLoginUserOutput>(AuthErrors.ContaBloqueada);
        }
        
        // 4. *** VERIFICAR SE O USUÁRIO TEM ROLE ADMIN ***
        var roles = await _identityService.GetRolesAsync(user.Id, cancellationToken);
        
        if (!roles.Contains(AdminRoleName))
        {
            // Registrar tentativa de acesso não autorizado
            var unauthorizedDeviceInfo = !string.IsNullOrEmpty(input.DeviceName)
                ? System.Text.Json.JsonSerializer.Serialize(new { userAgent = input.DeviceName })
                : null;

            var unauthorizedHistory = LoginHistory.CreateFailure(
                user.Id,
                "Acesso negado: usuário não é administrador",
                loginProvider: "AdminPanel",
                ipAddress: input.IpAddress,
                deviceType: input.DeviceType.ToString(),
                deviceInfo: unauthorizedDeviceInfo);

            await _loginHistoryRepository.AddAsync(unauthorizedHistory, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Failure<AdminLoginUserOutput>(AuthErrors.AcessoNaoAutorizado);
        }
        
        // 5. Buscar perfil para obter FirstName e LastName
        var profile = await _profileRepository.GetByUserIdAsync(user.Id, cancellationToken);
        
        // 6. Construir claims para o JWT
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, profile?.DisplayName ?? user.Email!)
        };
        
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
  
        // 7. Gerar tokens
        var accessTokenResult = _tokenGenerator.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshTokenResult = _tokenGenerator.GenerateRefreshToken();
        
        // 8. Persistir sessão (RefreshToken)
        var session = Session.Create(
            userId: user.Id,
            refreshTokenHash: refreshTokenResult.TokenHash,
            expiresAt: refreshTokenResult.ExpiresAt,
            deviceId: null, // DeviceId pode vir do input se o cliente enviar, ou null
            deviceName: input.DeviceName ?? "Unknown Device",
            deviceType: input.DeviceType.ToString(),
            ipAddress: input.IpAddress
        );

        await _sessionRepository.AddAsync(session, cancellationToken);
        
        // 9. Registrar histórico de login com sucesso
        var successHistory = LoginHistory.CreateSuccess(
            userId: user.Id,
            loginProvider: "Local",
            ipAddress: input.IpAddress,
            userAgent: input.DeviceName, // Usando DeviceName como UserAgent simplificado
            deviceType: input.DeviceType.ToString(),
            deviceInfo: !string.IsNullOrEmpty(input.DeviceName) 
                ? System.Text.Json.JsonSerializer.Serialize(new { userAgent = input.DeviceName }) 
                : null
        );

        await _loginHistoryRepository.AddAsync(successHistory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AdminLoginUserOutput(
            UserId: user.Id,
            Email: user.Email!,
            FirstName: profile?.FirstName,
            LastName: profile?.LastName,
            AccessToken: accessTokenResult.Token,
            RefreshToken: refreshTokenResult.Token,
            ExpiresAt: accessTokenResult.ExpiresAt
        ));
        
    }
}