using Microsoft.AspNetCore.Authorization;
using Shared.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Models;
using Users.Application.Features.Admin.AdminLogin;
using Users.Domain.Enums;

namespace Btree.Api.Controllers;

[Route("api/admin/auth")]
public class AdminController  : ApiControllerBase
{    
    
    IAdminLoginUserUseCase _adminLoginUserUseCase;

    public AdminController(IAdminLoginUserUseCase adminLoginUserUseCase)
    {
        _adminLoginUserUseCase = adminLoginUserUseCase;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AdminLoginUserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AdminLoginUserInput input, CancellationToken cancellationToken)
    {
        var ipAddress = GetClientIpAddress();
        var userAgent = GetUserAgent();
        var deviceType = ParseDeviceType(userAgent);

        var command = new AdminLoginUserInput(
            Email: input.Email,
            Password: input.Password,
            IpAddress: ipAddress,
            DeviceName: userAgent,
            DeviceType: deviceType);
        
        var result = await _adminLoginUserUseCase.ExecuteAsync(command, cancellationToken);
        
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