using FluentAssertions;
using Temp.Services.Results;

namespace Temp.Tests.Unit.Results;

public class ResultTests
{
    [Fact]
    public void Success_CreatesSuccessfulResult() {

        var result = Result.Success();


        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_CreatesFailedResult() {

        var error = "Operation failed";
        var errorCode = "OP_FAILED";


        var result = Result.Failure(error, errorCode);


        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        result.ErrorCode.Should().Be(errorCode);
    }

    [Fact]
    public void ValidationFailure_CreatesValidationFailedResult() {

        var error = "Validation failed";
        var validationErrors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } }
        };


        var result = Result.ValidationFailure(error, validationErrors);


        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("VALIDATION_ERROR");
        result.ValidationErrors.Should().ContainKey("Email");
    }

    [Fact]
    public void SuccessWithValue_CreatesSuccessfulResultWithValue() {

        var value = "Test Value";


        var result = Result.Success(value);


        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void FailureWithType_CreatesFailedResultOfType() {

        var error = "Not found";
        var errorCode = "NOT_FOUND";


        var result = Result.Failure<string>(error, errorCode);


        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        result.ErrorCode.Should().Be(errorCode);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void ValidationFailureWithType_CreatesValidationFailedResultOfType() {

        var error = "Validation failed";
        var validationErrors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } }
        };


        var result = Result.ValidationFailure<int>(error, validationErrors);


        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("VALIDATION_ERROR");
        result.ValidationErrors.Should().ContainKey("Name");
    }


    [Fact]
    public void Success_DoesNotHaveError() {

        var result = Result.Success();


        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_AlwaysHasError() {

        var result = Result.Failure("Error message");


        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Result_CanBeUsedInConditionals() {

        var successResult = Result.Success();
        var failureResult = Result.Failure("Error");


        if (successResult.IsSuccess) {
            Assert.True(true);
        } else {
            Assert.Fail("Should be success");
        }

        if (failureResult.IsFailure) {
            Assert.True(true);
        } else {
            Assert.Fail("Should be failure");
        }
    }

    [Fact]
    public void ResultOfT_WithValue_CanAccessValue() {

        var testObject = new { Id = 1, Name = "Test" };


        var result = Result.Success(testObject);


        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);
        result.Value.Name.Should().Be("Test");
    }
}