using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Temp.Services._Shared;

public static class Guard
{
    public static T NotNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : class {
        if (value is null)
            throw new ArgumentNullException(paramName);
        return value;
    }

    public static string NotNullOrEmpty(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) {
        if (value is null)
            throw new ArgumentNullException(paramName);
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty or whitespace.", paramName);
        return value;
    }

    public static string? NotEmpty(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) {
        if (value is not null && string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty or whitespace.", paramName);
        return value;
    }

    public static int Positive(
        int value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be positive.");
        return value;
    }

    public static int NotNegative(
        int value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be negative.");
        return value;
    }

    public static int InRange(
        int value,
        int min,
        int max,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.");
        return value;
    }

    public static ICollection<T> NotNullOrEmpty<T>(
        [NotNull] ICollection<T>? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) {
        if (value is null)
            throw new ArgumentNullException(paramName);
        if (value.Count == 0)
            throw new ArgumentException("Collection cannot be empty.", paramName);
        return value;
    }

    public static string MaxLength(
        string? value,
        int maxLength,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) {
        if (value is not null && value.Length > maxLength)
            throw new ArgumentException($"Value cannot exceed {maxLength} characters.", paramName);
        return value!;
    }

    public static void That(
        [DoesNotReturnIf(false)] bool condition,
        string message,
        [CallerArgumentExpression(nameof(condition))] string? paramName = null) {
        if (!condition)
            throw new ArgumentException(message, paramName);
    }

    public static void Ensure(
        [DoesNotReturnIf(false)] bool condition,
        string message) {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}