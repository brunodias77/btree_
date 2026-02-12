using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.ValueObjects;

/// <summary>
/// Value object representing geographic coordinates (latitude and longitude).
/// </summary>
public sealed class GeoCoordinates : ValueObject
{
    public decimal Latitude { get; }
    public decimal Longitude { get; }

    private GeoCoordinates(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static GeoCoordinates Create(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");

        return new GeoCoordinates(latitude, longitude);
    }

    public static GeoCoordinates? TryCreate(decimal? latitude, decimal? longitude)
    {
        if (!latitude.HasValue || !longitude.HasValue)
            return null;

        if (latitude.Value < -90 || latitude.Value > 90)
            return null;

        if (longitude.Value < -180 || longitude.Value > 180)
            return null;

        return new GeoCoordinates(latitude.Value, longitude.Value);
    }

    /// <summary>
    /// Calculates the distance in kilometers to another coordinate using the Haversine formula.
    /// </summary>
    public double DistanceTo(GeoCoordinates other)
    {
        const double earthRadiusKm = 6371;

        var dLat = ToRadians((double)(other.Latitude - Latitude));
        var dLon = ToRadians((double)(other.Longitude - Longitude));

        var lat1 = ToRadians((double)Latitude);
        var lat2 = ToRadians((double)other.Latitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }

    public override string ToString() => $"{Latitude:F6}, {Longitude:F6}";
}
