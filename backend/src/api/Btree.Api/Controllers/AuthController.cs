using Shared.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Models;
using Users.Application.Features.Auth.Register;
using Users.Application.Features.Auth.ConfirmEmail;
using Users.Application.Features.Auth.Login;
using Users.Application.Features.Auth.RefreshToken;
using Users.Application.Features.Auth.Logout;
using Users.Application.Features.Auth.ResendConfirmationEmail;
using Users.Application.Features.Auth.ForgotPassword;
using Users.Application.Features.Auth.ResetPassword;
using Users.Application.Features.Auth.ChangePassword;
using Users.Domain.Enums;

namespace Btree.Api.Controllers;

public record RefreshRequest(string RefreshToken);

[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IRegisterUserUseCase _registerUserUseCase;
    private readonly IConfirmEmailUseCase _confirmEmailUseCase;
    private readonly  ILoginUserUseCase _loginUserUseCase;
    private readonly IRefreshTokenUseCase _refreshTokenUseCase;
    private readonly ILogoutUseCase _logoutUseCase;
    private readonly IResendConfirmationEmailUseCase _resendConfirmationEmailUseCase;
    private readonly IForgotPasswordUseCase _forgotPasswordUseCase;
    private readonly IResetPasswordUseCase _resetPasswordUseCase;
    private readonly IChangePasswordUseCase _changePasswordUseCase;

    public AuthController(
        IRegisterUserUseCase registerUserUseCase,
        IConfirmEmailUseCase confirmEmailUseCase,
        ILoginUserUseCase loginUserUseCase,
        IRefreshTokenUseCase refreshTokenUseCase,
        ILogoutUseCase logoutUseCase,
        IResendConfirmationEmailUseCase resendConfirmationEmailUseCase,
        IForgotPasswordUseCase forgotPasswordUseCase,
        IResetPasswordUseCase resetPasswordUseCase,
        IChangePasswordUseCase changePasswordUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
        _confirmEmailUseCase = confirmEmailUseCase;
        _loginUserUseCase = loginUserUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
        _resendConfirmationEmailUseCase = resendConfirmationEmailUseCase;
        _forgotPasswordUseCase = forgotPasswordUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
        _changePasswordUseCase = changePasswordUseCase;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserInput request, CancellationToken cancellationToken)
    {
        var result = await _registerUserUseCase.ExecuteAsync(request, cancellationToken);
        
        return HandleResult(result);
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailInput request, CancellationToken cancellationToken)
    {
        var result = await _confirmEmailUseCase.ExecuteAsync(request, cancellationToken);
        
        return HandleResult(result);
    }

    [HttpPost("resend-confirmation-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailInput input, CancellationToken cancellationToken)
    {
        var result = await _resendConfirmationEmailUseCase.ExecuteAsync(input, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleResult(result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordInput input, CancellationToken cancellationToken)
    {
        var result = await _forgotPasswordUseCase.ExecuteAsync(input, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleResult(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordInput input, CancellationToken cancellationToken)
    {
        var result = await _resetPasswordUseCase.ExecuteAsync(input, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInput input, CancellationToken cancellationToken)
    {
        var result = await _changePasswordUseCase.ExecuteAsync(input, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginUserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserInput input, CancellationToken cancellationToken)
    {        
        var ipAddress = GetClientIpAddress();
        var userAgent = GetUserAgent();
        var deviceType = ParseDeviceType(userAgent);

        var command = new LoginUserInput(
            Email: input.Email,
            Password: input.Password,
            IpAddress: ipAddress,
            DeviceName: userAgent,
            DeviceType: deviceType);
        
        var result = await _loginUserUseCase.ExecuteAsync(command, cancellationToken);
        
        return HandleResult(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = GetClientIpAddress();
        var userAgent = GetUserAgent();

        var input = new RefreshTokenInput(
            RefreshToken: request.RefreshToken,
            IpAddress: ipAddress,
            DeviceInfo: userAgent
        );

        var result = await _refreshTokenUseCase.ExecuteAsync(input, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] LogoutInput input, CancellationToken cancellationToken)
    {
        var result = await _logoutUseCase.ExecuteAsync(input, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleResult(result);
    }
    
    
    #region Private Helpers

    private string GetClientIpAddress()
    {
        // Tenta obter IP do header X-Forwarded-For (quando atrás de proxy/load balancer)
        var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // X-Forwarded-For pode conter múltiplos IPs separados por vírgula
            return forwardedFor.Split(',')[0].Trim();
        }

        // Fallback para conexão direta
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }

    private string GetUserAgent()
    {
        var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "unknown";
        return userAgent.Length > 100 ? userAgent[..100] : userAgent;
    }

    private static DeviceType ParseDeviceType(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return DeviceType.Desconhecido;

        var ua = userAgent.ToLowerInvariant();

        if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
            return DeviceType.Mobile;

        if (ua.Contains("tablet") || ua.Contains("ipad"))
            return DeviceType.Tablet;

        if (ua.Contains("smart-tv") || ua.Contains("smarttv") || ua.Contains("tv"))
            return DeviceType.SmartTV;

        if (ua.Contains("windows") || ua.Contains("macintosh") || ua.Contains("linux"))
            return DeviceType.Desktop;

        return DeviceType.Outro;
    }

    #endregion
}