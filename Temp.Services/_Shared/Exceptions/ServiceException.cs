namespace Temp.Services.Exceptions;

/// <summary>
/// Base exception for all service layer exceptions
/// </summary>
public abstract class ServiceException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, string[]>? ValidationErrors { get; }

    protected ServiceException(string message, string errorCode, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    protected ServiceException(string message, string errorCode, Dictionary<string, string[]> validationErrors)
        : base(message)
    {
        ErrorCode = errorCode;
        ValidationErrors = validationErrors;
    }
}

/// <summary>
/// Exception thrown when input validation fails
/// </summary>
public class ValidationException : ServiceException
{
    public ValidationException(string message, Dictionary<string, string[]> validationErrors)
        : base(message, "VALIDATION_ERROR", validationErrors)
    {
    }

    public ValidationException(string message, string fieldName, string error)
        : base(message, "VALIDATION_ERROR", new Dictionary<string, string[]>
        {
            { fieldName, new[] { error } }
        })
    {
    }
}

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : ServiceException
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found", "NOT_FOUND")
    {
    }

    public NotFoundException(string message)
        : base(message, "NOT_FOUND")
    {
    }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleException : ServiceException
{
    public BusinessRuleException(string message)
        : base(message, "BUSINESS_RULE_VIOLATION")
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message, "BUSINESS_RULE_VIOLATION", innerException)
    {
    }
}

/// <summary>
/// Exception thrown when database or external dependency fails
/// </summary>
public class DependencyException : ServiceException
{
    public DependencyException(string message, Exception innerException)
        : base(message, "DEPENDENCY_ERROR", innerException)
    {
    }
}

/// <summary>
/// Exception thrown when an operation is not authorized
/// </summary>
public class UnauthorizedAccessException : ServiceException
{
    public UnauthorizedAccessException(string message)
        : base(message, "UNAUTHORIZED")
    {
    }
}

/// <summary>
/// Exception thrown when a conflict occurs (e.g., duplicate key)
/// </summary>
public class ConflictException : ServiceException
{
    public ConflictException(string message)
        : base(message, "CONFLICT")
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, "CONFLICT", innerException)
    {
    }
}
