using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando um valor monetário.
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>
    /// Valor numérico do dinheiro.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Código da moeda (ex: BRL, USD).
    /// </summary>
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Cria um novo valor monetário em Reais (BRL).
    /// </summary>
    public static Money InBRL(decimal amount)
    {
        return Create(amount, "BRL");
    }

    /// <summary>
    /// Cria um novo valor monetário.
    /// </summary>
    /// <param name="amount">Valor.</param>
    /// <param name="currency">Código da moeda.</param>
    /// <returns>Nova instância de Money.</returns>
    /// <exception cref="ArgumentException">Quando os valores são inválidos.</exception>
    public static Money Create(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new ArgumentException("O valor não pode ser negativo.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("A moeda é obrigatória.", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("O código da moeda deve ter 3 caracteres.", nameof(currency));

        return new Money(Math.Round(amount, 2), currency.ToUpperInvariant());
    }

    /// <summary>
    /// Cria um valor monetário zero.
    /// </summary>
    public static Money Zero(string currency = "BRL") => Create(0, currency);

    /// <summary>
    /// Adiciona dois valores monetários.
    /// </summary>
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Subtrai um valor monetário.
    /// </summary>
    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = Amount - other.Amount;
        
        if (result < 0)
            throw new InvalidOperationException("O resultado não pode ser negativo.");

        return new Money(result, Currency);
    }

    /// <summary>
    /// Multiplica por uma quantidade.
    /// </summary>
    public Money Multiply(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("A quantidade não pode ser negativa.", nameof(quantity));

        return new Money(Amount * quantity, Currency);
    }

    /// <summary>
    /// Aplica desconto percentual.
    /// </summary>
    /// <param name="percentageDiscount">Desconto em percentual (0-100).</param>
    public Money ApplyDiscount(decimal percentageDiscount)
    {
        if (percentageDiscount < 0 || percentageDiscount > 100)
            throw new ArgumentException("O desconto deve estar entre 0 e 100.", nameof(percentageDiscount));

        var discountAmount = Amount * (percentageDiscount / 100);
        return new Money(Math.Round(Amount - discountAmount, 2), Currency);
    }

    /// <summary>
    /// Verifica se o valor é maior que outro.
    /// </summary>
    public bool IsGreaterThan(Money other)
    {
        EnsureSameCurrency(other);
        return Amount > other.Amount;
    }

    /// <summary>
    /// Verifica se o valor é zero.
    /// </summary>
    public bool IsZero() => Amount == 0;

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Não é possível operar valores em moedas diferentes: {Currency} e {other.Currency}.");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:N2}";

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, int quantity) => money.Multiply(quantity);
    public static bool operator >(Money left, Money right) => left.IsGreaterThan(right);
    public static bool operator <(Money left, Money right) => right.IsGreaterThan(left);
}
