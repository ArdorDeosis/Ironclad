using System.Diagnostics.CodeAnalysis;

namespace Ironclad.ResultTypes;

/// <summary>
/// A result type containing data. Provides information about whether an operation was successful or not and either the
/// result data in case of a success or an error message in case of failure.
/// </summary>
public sealed class Result<T> where T : notnull
{
  /// <summary>
  /// The result data.
  /// </summary>
  /// <remarks>This is only set if the result indicates a success otherwise it the default value.</remarks>
  public T? Data { get; private init; }

  /// <summary>
  /// The error message providing more detail about what went wrong.
  /// </summary>
  /// <remarks>This is only set if the result indicates an error otherwise it is null.</remarks>
  public string? ErrorMessage { get; private init; }

  /// <summary>
  /// Whether the result indicates a success.
  /// </summary>
  [MemberNotNullWhen(true, nameof(Data))]
  [MemberNotNullWhen(false, nameof(ErrorMessage))]
  public bool IsSuccess { get; private init; }

  /// <summary>
  /// Whether the result indicates an error.
  /// </summary>
  [MemberNotNullWhen(false, nameof(Data))]
  [MemberNotNullWhen(true, nameof(ErrorMessage))]
  public bool IsError => !IsSuccess;

  private Result() { }

  /// <summary>
  /// Implicit conversion of a string to a result indicating an error.
  /// </summary>
  /// <param name="errorMessage">The error message providing more information about what went wrong.</param>
  public static implicit operator Result<T>(string errorMessage) => Error(errorMessage);

  /// <summary>
  /// Implicit conversion of an object of the result type to a result indicating a success.
  /// </summary>
  /// <param name="value">The result value.</param>
  public static implicit operator Result<T>(T value) => Success(value);

  /// <summary>
  /// Implicit conversion of a result with value to a result without value.
  /// </summary>
  /// <param name="result">The result with value.</param>
  public static implicit operator Result(Result<T> result) => result.IsSuccess
    ? Result.Success
    : Result.Error(result.ErrorMessage);
    
  /// <summary>
  /// Creates a result indicating a success.
  /// </summary>
  /// <param name="value">The result value.</param>
  public static Result<T> Success(T value) => new() { Data = value, IsSuccess = true };

  /// <summary>
  /// A result indicating an error.
  /// </summary>
  /// <param name="errorMessage">The error message providing more information about what went wrong.</param>
  public static Result<T> Error(string errorMessage) => new() { ErrorMessage = errorMessage, IsSuccess = false };
}