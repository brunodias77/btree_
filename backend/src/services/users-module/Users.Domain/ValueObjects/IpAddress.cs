using System.Net;
using System.Text.RegularExpressions;
using BuildingBlocks.Domain.Abstractions;


namespace Users.Domain.ValueObjects;

/// <summary>
/// Value object representing an IP address (IPv4 or IPv6).
/// </summary>
public sealed class IpAddress : ValueObject
{
    private static readonly Regex Ipv4Regex = new(
        @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
        RegexOptions.Compiled);

    public string Value { get; }
    public bool IsIPv4 { get; }
    public bool IsIPv6 { get; }
    public bool IsLoopback { get; }
    public bool IsPrivate { get; }

    private IpAddress(string value)
    {
        Value = value;

        if (System.Net.IPAddress.TryParse(value, out var ipAddress))
        {
            IsIPv4 = ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
            IsIPv6 = ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            IsLoopback = System.Net.IPAddress.IsLoopback(ipAddress);
            IsPrivate = IsPrivateIpAddress(ipAddress);
        }
    }

    public static IpAddress Create(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be empty.", nameof(ipAddress));

        if (!System.Net.IPAddress.TryParse(ipAddress.Trim(), out _))
            throw new ArgumentException("Invalid IP address format.", nameof(ipAddress));

        return new IpAddress(ipAddress.Trim());
    }

    public static IpAddress? TryCreate(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return null;

        return System.Net.IPAddress.TryParse(ipAddress.Trim(), out _) 
            ? new IpAddress(ipAddress.Trim()) 
            : null;
    }

    public static bool IsValid(string ipAddress)
    {
        return !string.IsNullOrWhiteSpace(ipAddress) && 
               System.Net.IPAddress.TryParse(ipAddress.Trim(), out _);
    }

    private static bool IsPrivateIpAddress(System.Net.IPAddress ipAddress)
    {
        var bytes = ipAddress.GetAddressBytes();

        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            // 10.0.0.0 - 10.255.255.255
            if (bytes[0] == 10)
                return true;

            // 172.16.0.0 - 172.31.255.255
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                return true;

            // 192.168.0.0 - 192.168.255.255
            if (bytes[0] == 192 && bytes[1] == 168)
                return true;
        }

        return false;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(IpAddress ipAddress) => ipAddress.Value;
}
