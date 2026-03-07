using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando as dimensões físicas de um produto.
/// </summary>
public sealed class Dimensions : ValueObject
{
    /// <summary>
    /// Peso em gramas.
    /// </summary>
    public int? WeightGrams { get; }

    /// <summary>
    /// Altura em centímetros.
    /// </summary>
    public decimal? HeightCm { get; }

    /// <summary>
    /// Largura em centímetros.
    /// </summary>
    public decimal? WidthCm { get; }

    /// <summary>
    /// Comprimento em centímetros.
    /// </summary>
    public decimal? LengthCm { get; }

    private Dimensions(int? weightGrams, decimal? heightCm, decimal? widthCm, decimal? lengthCm)
    {
        WeightGrams = weightGrams;
        HeightCm = heightCm;
        WidthCm = widthCm;
        LengthCm = lengthCm;
    }

    /// <summary>
    /// Cria novas dimensões.
    /// </summary>
    public static Dimensions Create(
        int? weightGrams = null,
        decimal? heightCm = null,
        decimal? widthCm = null,
        decimal? lengthCm = null)
    {
        if (weightGrams.HasValue && weightGrams <= 0)
            throw new ArgumentException("O peso deve ser maior que zero.", nameof(weightGrams));

        if (heightCm.HasValue && heightCm <= 0)
            throw new ArgumentException("A altura deve ser maior que zero.", nameof(heightCm));

        if (widthCm.HasValue && widthCm <= 0)
            throw new ArgumentException("A largura deve ser maior que zero.", nameof(widthCm));

        if (lengthCm.HasValue && lengthCm <= 0)
            throw new ArgumentException("O comprimento deve ser maior que zero.", nameof(lengthCm));

        return new Dimensions(weightGrams, heightCm, widthCm, lengthCm);
    }

    /// <summary>
    /// Cria dimensões vazias.
    /// </summary>
    public static Dimensions Empty() => new(null, null, null, null);

    /// <summary>
    /// Peso em quilogramas.
    /// </summary>
    public decimal? WeightKg => WeightGrams.HasValue ? WeightGrams.Value / 1000m : null;

    /// <summary>
    /// Volume em cm³.
    /// </summary>
    public decimal? VolumeCm3 => 
        HeightCm.HasValue && WidthCm.HasValue && LengthCm.HasValue 
            ? HeightCm.Value * WidthCm.Value * LengthCm.Value 
            : null;

    /// <summary>
    /// Verifica se todas as dimensões estão preenchidas.
    /// </summary>
    public bool IsComplete => 
        WeightGrams.HasValue && HeightCm.HasValue && WidthCm.HasValue && LengthCm.HasValue;

    /// <summary>
    /// Verifica se está vazio.
    /// </summary>
    public bool IsEmpty => 
        !WeightGrams.HasValue && !HeightCm.HasValue && !WidthCm.HasValue && !LengthCm.HasValue;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return WeightGrams;
        yield return HeightCm;
        yield return WidthCm;
        yield return LengthCm;
    }

    public override string ToString()
    {
        var parts = new List<string>();
        
        if (WeightGrams.HasValue)
            parts.Add($"{WeightGrams}g");
        
        if (HeightCm.HasValue && WidthCm.HasValue && LengthCm.HasValue)
            parts.Add($"{LengthCm}x{WidthCm}x{HeightCm}cm");

        return parts.Count > 0 ? string.Join(" | ", parts) : "Sem dimensões";
    }
}
