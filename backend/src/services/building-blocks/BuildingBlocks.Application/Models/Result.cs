namespace BuildingBlocks.Application.Models;
/// <summary>
/// Representa um erro de aplicação.
/// </summary>
/// <param name="Code">Código único do erro para identificação programática.</param>
/// <param name="Message">Mensagem descritiva do erro.</param>
public sealed record Error(string Code, string Message)
{
    /// <summary>
    /// Erro nulo/vazio, representa ausência de erro.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Erro genérico de operação nula.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "O valor fornecido é nulo.");

    /// <summary>
    /// Erro de item não encontrado.
    /// </summary>
    public static Error NotFound(string entity, object id) =>
        new($"{entity}.NotFound", $"{entity} com ID '{id}' não foi encontrado.");

    /// <summary>
    /// Erro de item não encontrado com código e mensagem customizados.
    /// </summary>
    public static Error NotFound(string code, string message) =>
        new(code, message);

    /// <summary>
    /// Erro de conflito/duplicação.
    /// </summary>
    public static Error Conflict(string entity, string reason) =>
        new($"{entity}.Conflict", reason);

    /// <summary>
    /// Erro de validação.
    /// </summary>
    public static Error Validation(string field, string message) =>
        new($"Validation.{field}", message);

    /// <summary>
    /// Erro de falha genérica.
    /// </summary>
    public static Error Failure(string code, string message) =>
        new(code, message);

    /// <summary>
    /// Erro de não autorizado.
    /// </summary>
    public static Error Unauthorized(string code = "Auth.Unauthorized", string message = "Não autorizado.") =>
        new(code, message);

    /// <summary>
    /// Erro de acesso negado.
    /// </summary>
    public static Error Forbidden(string code = "Auth.Forbidden", string message = "Acesso negado.") =>
        new(code, message);
}

/// <summary>
/// Representa o resultado de uma operação que pode ter sucesso ou falha.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Resultado de sucesso não pode ter erro.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Resultado de falha deve ter um erro.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Cria um resultado de sucesso.
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Cria um resultado de falha.
    /// </summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Cria um resultado de sucesso com valor.
    /// </summary>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>
    /// Cria um resultado de falha tipado.
    /// </summary>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    /// <summary>
    /// Cria resultado baseado em condição.
    /// </summary>
    public static Result Create(bool condition, Error error) =>
        condition ? Success() : Failure(error);

    /// <summary>
    /// Cria resultado baseado em valor nullable.
    /// </summary>
    public static Result<TValue> Create<TValue>(TValue? value, Error error) where TValue : class =>
        value is not null ? Success(value) : Failure<TValue>(error);
}

/// <summary>
/// Representa o resultado de uma operação que retorna um valor tipado.
/// </summary>
/// <typeparam name="TValue">Tipo do valor retornado.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Valor do resultado. Lança exceção se acessado em resultado de falha.
    /// </summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possível acessar o valor de um resultado com falha.");

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Conversão implícita de valor para Result de sucesso.
    /// </summary>
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    /// <summary>
    /// Mapeia o valor para outro tipo.
    /// </summary>
    public Result<TOut> Map<TOut>(Func<TValue, TOut> mapper)
    {
        return IsSuccess
            ? Success(mapper(Value))
            : Failure<TOut>(Error);
    }

    /// <summary>
    /// Executa ação se sucesso.
    /// </summary>
    public Result<TValue> OnSuccess(Action<TValue> action)
    {
        if (IsSuccess) action(Value);
        return this;
    }

    /// <summary>
    /// Executa ação se falha.
    /// </summary>
    public Result<TValue> OnFailure(Action<Error> action)
    {
        if (IsFailure) action(Error);
        return this;
    }

    /// <summary>
    /// Retorna valor ou default se falha.
    /// </summary>
    public TValue? ValueOrDefault() => IsSuccess ? Value : default;

    /// <summary>
    /// Retorna valor ou alternativa se falha.
    /// </summary>
    public TValue ValueOrElse(TValue defaultValue) => IsSuccess ? Value : defaultValue;
}
