using FluentAssertions;
using Temp.Services.Exceptions;

namespace Temp.Tests.Unit.Exceptions;

public class ServiceExceptionTests
{
    [Fact]
    public void ValidationException_WithFieldErrors_SetsPropertiesCorrectly() {
        var fieldName = "Email";
        var error = "Email is required";

        var exception = new ValidationException("Validation failed", fieldName, error);

        exception.Message.Should().Be("Validation failed");
        exception.ErrorCode.Should().Be("VALIDATION_ERROR");
        exception.ValidationErrors.Should().ContainKey(fieldName);
        exception.ValidationErrors[fieldName].Should().Contain(error);
    }

    [Fact]
    public void ValidationException_WithMultipleErrors_SetsAllErrors() {
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required", "Email format is invalid" } },
            { "Password", new[] { "Password is too short" } }
        };


        var exception = new ValidationException("Validation failed", errors);

        exception.ValidationErrors.Should().HaveCount(2);
        exception.ValidationErrors["Email"].Should().HaveCount(2);
        exception.ValidationErrors["Password"].Should().HaveCount(1);
    }

    [Fact]
    public void NotFoundException_WithResourceAndKey_FormatsMessageCorrectly() {
        var resourceName = "Employee";
        var key = 123;

        var exception = new NotFoundException(resourceName, key);

        exception.Message.Should().Be("Employee with key '123' was not found");
        exception.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void NotFoundException_WithCustomMessage_UsesProvidedMessage() {
        var message = "Custom not found message";

        var exception = new NotFoundException(message);

        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void BusinessRuleException_WithMessage_SetsPropertiesCorrectly() {
        var message = "Cannot delete employee with active engagements";

        var exception = new BusinessRuleException(message);

        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("BUSINESS_RULE_VIOLATION");
    }

    [Fact]
    public void DependencyException_WithInnerException_PreservesInnerException() {
        var innerException = new Exception("Database connection failed");
        var message = "Failed to access database";

        var exception = new DependencyException(message, innerException);

        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("DEPENDENCY_ERROR");
        exception.InnerException.Should().Be(innerException);
        exception.InnerException!.Message.Should().Be("Database connection failed");
    }

    [Fact]
    public void UnauthorizedAccessException_WithMessage_SetsPropertiesCorrectly() {
        var message = "User does not have permission to perform this action";

        var exception = new Temp.Services.Exceptions.UnauthorizedAccessException(message);

        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("UNAUTHORIZED");
    }

    [Fact]
    public void ConflictException_WithMessage_SetsPropertiesCorrectly() {
        var message = "Employee with this email already exists";

        var exception = new ConflictException(message);

        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("CONFLICT");
    }

    [Fact]
    public void ConflictException_WithInnerException_PreservesInnerException() {

        var innerException = new Exception("Unique constraint violation");
        var message = "Duplicate entry detected";

        var exception = new ConflictException(message, innerException);

        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("CONFLICT");
        exception.InnerException.Should().NotBeNull();
    }
}