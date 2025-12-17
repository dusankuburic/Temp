using FluentAssertions;
using Temp.Services.Results;

namespace Temp.Tests.Unit.Results;

public class ResultTests
{
    [Fact]
    public void Success_CreatesSuccessfulResult() {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_CreatesFailedResult() {
        // Arrange
        var error = "Operation failed";
        var errorCode = "OP_FAILED";

        // Act
        var result = Result.Failure(error, errorCode);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        result.ErrorCode.Should().Be(errorCode);
    }

    [Fact]
    public void ValidationFailure_CreatesValidationFailedResult() {
        // Arrange
        var error = "Validation failed";
        var validationErrors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } }
        };

        // Act
        var result = Result.ValidationFailure(error, validationErrors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("VALIDATION_ERROR");
        result.ValidationErrors.Should().ContainKey("Email");
    }

    [Fact]
    public void SuccessWithValue_CreatesSuccessfulResultWithValue() {
        // Arrange
        var value = "Test Value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void FailureWithType_CreatesFailedResultOfType() {
        // Arrange
        var error = "Not found";
        var errorCode = "NOT_FOUND";

        // Act
        var result = Result.Failure<string>(error, errorCode);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        result.ErrorCode.Should().Be(errorCode);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void ValidationFailureWithType_CreatesValidationFailedResultOfType() {
        // Arrange
        var error = "Validation failed";
        var validationErrors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } }
        };

        // Act
        var result = Result.ValidationFailure<int>(error, validationErrors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("VALIDATION_ERROR");
        result.ValidationErrors.Should().ContainKey("Name");
    }

    // Note: Constructor is protected, tested through public API
    [Fact]
    public void Success_DoesNotHaveError() {
        // Act
        var result = Result.Success();

        // Assert
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_AlwaysHasError() {
        // Act
        var result = Result.Failure("Error message");

        // Assert
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Result_CanBeUsedInConditionals() {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure("Error");

        // Act & Assert
        if (successResult.IsSuccess) {
            Assert.True(true); // Success path
        } else {
            Assert.Fail("Should be success");
        }

        if (failureResult.IsFailure) {
            Assert.True(true); // Failure path
        } else {
            Assert.Fail("Should be failure");
        }
    }

    [Fact]
    public void ResultOfT_WithValue_CanAccessValue() {
        // Arrange
        var testObject = new { Id = 1, Name = "Test" };

        // Act
        var result = Result.Success(testObject);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);
        result.Value.Name.Should().Be("Test");
    }
}
