using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.ValueObjects;

/// <summary>
/// Value object representing device information for login/session tracking.
/// </summary>
public sealed class DeviceInfo : ValueObject
{
    public string? DeviceId { get; }
    public string? DeviceName { get; }
    public string? DeviceType { get; }
    public string? UserAgent { get; }
    public string? OperatingSystem { get; }
    public string? Browser { get; }

    private DeviceInfo(
        string? deviceId,
        string? deviceName,
        string? deviceType,
        string? userAgent,
        string? operatingSystem,
        string? browser)
    {
        DeviceId = deviceId;
        DeviceName = deviceName;
        DeviceType = deviceType;
        UserAgent = userAgent;
        OperatingSystem = operatingSystem;
        Browser = browser;
    }

    public static DeviceInfo Create(
        string? deviceId = null,
        string? deviceName = null,
        string? deviceType = null,
        string? userAgent = null,
        string? operatingSystem = null,
        string? browser = null)
    {
        return new DeviceInfo(deviceId, deviceName, deviceType, userAgent, operatingSystem, browser);
    }

    public static DeviceInfo FromUserAgent(string userAgent)
    {
        var deviceType = DetermineDeviceType(userAgent);
        var browser = DetermineBrowser(userAgent);
        var os = DetermineOperatingSystem(userAgent);

        return new DeviceInfo(null, null, deviceType, userAgent, os, browser);
    }

    private static string DetermineDeviceType(string userAgent)
    {
        var ua = userAgent.ToLowerInvariant();

        if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
            return "Mobile";

        if (ua.Contains("tablet") || ua.Contains("ipad"))
            return "Tablet";

        return "Desktop";
    }

    private static string DetermineBrowser(string userAgent)
    {
        var ua = userAgent.ToLowerInvariant();

        if (ua.Contains("edg"))
            return "Edge";
        if (ua.Contains("chrome"))
            return "Chrome";
        if (ua.Contains("firefox"))
            return "Firefox";
        if (ua.Contains("safari"))
            return "Safari";
        if (ua.Contains("opera"))
            return "Opera";

        return "Unknown";
    }

    private static string DetermineOperatingSystem(string userAgent)
    {
        var ua = userAgent.ToLowerInvariant();

        if (ua.Contains("windows"))
            return "Windows";
        if (ua.Contains("mac os") || ua.Contains("macintosh"))
            return "macOS";
        if (ua.Contains("linux"))
            return "Linux";
        if (ua.Contains("android"))
            return "Android";
        if (ua.Contains("iphone") || ua.Contains("ipad"))
            return "iOS";

        return "Unknown";
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DeviceId;
        yield return DeviceName;
        yield return DeviceType;
        yield return UserAgent;
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(DeviceName))
            return DeviceName;

        if (!string.IsNullOrEmpty(DeviceType) && !string.IsNullOrEmpty(Browser))
            return $"{DeviceType} - {Browser}";

        return DeviceType ?? "Unknown Device";
    }
}
