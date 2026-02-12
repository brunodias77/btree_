namespace BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Exceção base para erros de domínio.
/// Representa violações de regras de negócio que devem ser tratadas de forma específica.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Código do erro para identificação programática.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Detalhes adicionais sobre o erro (opcional).
    /// </summary>
    public IReadOnlyDictionary<string, object>? Details { get; }

    public DomainException(string message)
        : base(message)
    {
        Code = "DOMAIN_ERROR";
    }

    public DomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public DomainException(string code, string message, IDictionary<string, object> details)
        : base(message)
    {
        Code = code;
        Details = details.AsReadOnly();
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
        Code = "DOMAIN_ERROR";
    }

    public DomainException(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}

/// <summary>
/// Exceção lançada quando uma entidade não é encontrada.
/// </summary>
public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityName, object entityId)
        : base("ENTITY_NOT_FOUND", $"{entityName} com ID '{entityId}' não foi encontrado.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}

/// <summary>
/// Exceção lançada quando há conflito de concorrência (optimistic locking).
/// Geralmente indica que outro processo modificou o registro.
/// </summary>
public class ConcurrencyException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }
    public int ExpectedVersion { get; }

    public ConcurrencyException(string entityName, object entityId, int expectedVersion)
        : base("CONCURRENCY_CONFLICT", 
               $"Conflito de concorrência ao atualizar {entityName} com ID '{entityId}'. " +
               $"O registro foi modificado por outro processo. Versão esperada: {expectedVersion}.")
    {
        EntityName = entityName;
        EntityId = entityId;
        ExpectedVersion = expectedVersion;
    }
}

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada.
/// </summary>
public class BusinessRuleException : DomainException
{
    public string RuleName { get; }

    public BusinessRuleException(string ruleName, string message)
        : base("BUSINESS_RULE_VIOLATION", message)
    {
        RuleName = ruleName;
    }
}

/// <summary>
/// Exceção lançada quando há um erro de validação no domínio.
/// </summary>
public class DomainValidationException : DomainException
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public DomainValidationException(string message, IEnumerable<ValidationError> errors)
        : base("VALIDATION_ERROR", message)
    {
        Errors = errors.ToList().AsReadOnly();
    }

    public DomainValidationException(string field, string message)
        : base("VALIDATION_ERROR", message)
    {
        Errors = new List<ValidationError> { new(field, message) }.AsReadOnly();
    }
}

/// <summary>
/// Representa um erro de validação específico.
/// </summary>
public record ValidationError(string Field, string Message);
