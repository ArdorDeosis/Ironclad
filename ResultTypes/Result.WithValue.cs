using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Ironclad.ResultTypes;

/// <summary>
/// Represents a result that can either be a success with a value of type <typeparamref name="TValue"/>,
/// or a failure with a value of type <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value in case of success.</typeparam>
/// <typeparam name="TError">The type of the value in case of failure.</typeparam>
[PublicAPI]
public class Result<TValue, TError>
{
  private readonly bool success;
  private readonly TValue value;
  private readonly TError error;

  /// <summary>
  /// Initializes a new instance of the <see cref="Result{TValue, TError}"/> class with the specified success status,
  /// value in case of success, and error value in case of error.
  /// </summary>
  /// <param name="success">A value indicating whether the result is a success.</param>
  /// <param name="value">The value in case of success.</param>
  /// <param name="error">The value in case of error.</param>
  private Result(bool success, TValue value, TError error)
  {
    this.success = success;
    this.value = value;
    this.error = error;
  }

  /// <summary>
  /// Implicitly converts a successful <see cref="Result{TValue,TError}"/> to its value.
  /// </summary>
  /// <param name="result">The result to convert.</param>
  /// <returns>The value of the result if it is successful.</returns>
  /// <exception cref="InvalidOperationException">Thrown if the result is not a success.</exception>
  public static implicit operator TValue(Result<TValue, TError> result) => result.success
    ? result.value
    : throw new InvalidOperationException("Cannot convert a failed result to a value.");
  
  /// <summary>
  /// Implicitly converts a successful <see cref="Result{TValue,TError}"/> to a <see cref="Result{TError}"/>.
  /// </summary>
  /// <param name="result">The result to convert.</param>
  public static implicit operator Result<TError>(Result<TValue, TError> result) => result.success
    ? Result<TError>.Success
    : result.error;

  /// <summary>
  /// Implicitly converts a value to a successful <see cref="Result{TValue,TError}"/>.
  /// </summary>
  /// <param name="value">The value to convert.</param>
  /// <returns>A successful result with the specified value.</returns>
  public static implicit operator Result<TValue, TError>(TValue value) => Success(value);

  /// <summary>
  /// Implicitly converts a value to an unsuccessful <see cref="Result{TValue,TError}"/>.
  /// </summary>
  /// <param name="error">The error to convert.</param>
  /// <returns>An unsuccessful result with the specified error.</returns>
  public static implicit operator Result<TValue, TError>(TError error) => Error(error);

  /// <summary>
  /// Creates a new successful <see cref="Result{TValue,TError}"/> with the specified value.
  /// </summary>
  /// <param name="value">The value of the result.</param>
  /// <returns>A successful result with the specified value.</returns>
  public static Result<TValue, TError> Success(TValue value) => new(true, value, default!);

  /// <summary>
  /// Creates a new unsuccessful <see cref="Result{TValue,TError}"/> with the specified error.
  /// </summary>
  /// <param name="error">The error of the result.</param>
  /// <returns>An unsuccessful result with the specified error.</returns>
  public static Result<TValue, TError> Error(TError error) => new(false, default!, error);

  /// <summary>
  /// Gets the value of the result if it is successful, or the specified fallback value otherwise.
  /// </summary>
  /// <param name="fallback">The value to return if the result is not successful.</param>
  /// <returns>The value of the result if it is successful, or the specified fallback value otherwise.</returns>
  public TValue Or(TValue fallback) => success ? value : fallback;

  /// <summary>
  /// Gets the value of the result if it is successful, or the default value of the value type otherwise.
  /// </summary>
  /// <remarks>
  /// This property returns <c>null</c> if the value type is a reference type.
  /// </remarks>
  public TValue? OrDefault => success ? value : default;

  /// <summary>
  /// Gets the value of the result if it is successful, or throws the specified exception otherwise.
  /// </summary>
  public TValue OrThrow(Exception exception) => success ? value : throw exception;

  /// <summary>
  /// Indicates whether the result is an error.
  /// </summary>
  public bool IsError() => IsError(out _);

  /// <summary>
  /// Indicates whether the result is an error and retrieves the error value.
  /// </summary>
  /// <param name="error">The error value of the result.</param>
  /// <returns><c>true</c> if the result is an error; otherwise, <c>false</c>.</returns>
  [SuppressMessage("ReSharper", "ParameterHidesMember")]
  public bool IsError([NotNullWhen(true)] out TError? error)
  {
    error = this.error;
    return !success;
  }
}