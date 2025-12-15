using FluentAssertions;
using Temp.Services.Exceptions;
using Xunit;

namespace Temp.Tests.Unit.Exceptions;

public class ServiceExceptionTests
{
    [Fact]
    public void ValidationException_WithFieldErrors_SetsPropertiesCorrectly()
    {
        // Arrange
        var fieldName = "Email";
        var error = "Email is required";

        // Act
        var exception = new ValidationException("Validation failed", fieldName, error);

        // Assert
        exception.Message.Should().Be("Validation failed");
        exception.ErrorCode.Should().Be("VALIDATION_ERROR");
        exception.ValidationErrors.Should().ContainKey(fieldName);
        exception.ValidationErrors[fieldName].Should().Contain(error);
    }

    [Fact]
    public void ValidationException_WithMultipleErrors_SetsAllErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required", "Email format is invalid" } },
            { "Password", new[] { "Password is too short" } }
        };

        // Act
        var exception = new ValidationException("Validation failed", errors);

        // Assert
        exception.ValidationErrors.Should().HaveCount(2);
        exception.ValidationErrors["Email"].Should().HaveCount(2);
        exception.ValidationErrors["Password"].Should().HaveCount(1);
    }

    [Fact]
    public void NotFoundException_WithResourceAndKey_FormatsMessageCorrectly()
    {
        // Arrange
        var resourceName = "Employee";
        var key = 123;

        // Act
        var exception = new NotFoundException(resourceName, key);

        // Assert
        exception.Message.Should().Be("Employee with key '123' was not found");
        exception.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void NotFoundException_WithCustomMessage_UsesProvidedMessage()
    {
        // Arrange
        var message = "Custom not found message";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void BusinessRuleException_WithMessage_SetsPropertiesCorrectly()
    {
        // Arrange
        var message = "Cannot delete employee with active engagements";

        // Act
        var exception = new BusinessRuleException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("BUSINESS_RULE_VIOLATION");
    }

    [Fact]
    public void DependencyException_WithInnerException_PreservesInnerException()
    {
        // Arrange
        var innerException = new Exception("Database connection failed");
        var message = "Failed to access database";

        // Act
        var exception = new DependencyException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("DEPENDENCY_ERROR");
        exception.InnerException.Should().Be(innerException);
        exception.InnerException!.Message.Should().Be("Database connection failed");
    }

    [Fact]
    public void UnauthorizedAccessException_WithMessage_SetsPropertiesCorrectly()
    {
        // Arrange
        var message = "User does not have permission to perform this action";

        // Act
        var exception = new Temp.Services.Exceptions.UnauthorizedAccessException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("UNAUTHORIZED");
    }

    [Fact]
    public void ConflictException_WithMessage_SetsPropertiesCorrectly()
    {
        // Arrange
        var message = "Employee with this email already exists";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("CONFLICT");
    }

    [Fact]
    public void ConflictException_WithInnerException_PreservesInnerException()
    {
        // Arrange
        var innerException = new Exception("Unique constraint violation");
        var message = "Duplicate entry detected";

        // Act
        var exception = new ConflictException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("CONFLICT");
        exception.InnerException.Should().NotBeNull();
    }
}
