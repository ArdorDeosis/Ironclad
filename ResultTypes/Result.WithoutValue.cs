using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using JetBrains.Annotations;

namespace Ironclad.ResultTypes;

/// <summary>
/// Represents the result of an operation that may produce an error of type <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TError">The type of the error.</typeparam>
/// <remarks>Can implicitly be converted to a <see cref="bool"/>, indicating whether the operation succeeded.</remarks>
[PublicAPI]
public class Result<TError>
{
  private readonly bool success;
  private readonly TError error;

  /// <summary>
  /// Initializes a new instance of the <see cref="Result{TError}"/> class.
  /// </summary>
  /// <param name="success">A value indicating whether the operation succeeded.</param>
  /// <param name="error">The error produced by the operation, if any.</param>
  private Result(bool success, TError error)
  {
    this.success = success;
    this.error = error;
  }

  /// <summary>
  /// Implicitly converts a <see cref="Result{TError}"/> to a <see cref="bool"/>, indicating whether the operation
  /// succeeded.
  /// </summary>
  /// <param name="result">The result to convert.</param>
  public static implicit operator bool(Result<TError> result) => result.success;

  /// <summary>
  /// Implicitly converts an error of type <typeparamref name="TError"/> to a <see cref="Result{TError}"/> with a
  /// failure status.
  /// </summary>
  /// <param name="error">The error to convert.</param>
  public static implicit operator Result<TError>(TError error) => new(false, error);

  /// <summary>
  /// Creates a <see cref="Result{TError}"/> with a success status.
  /// </summary>
  public static Result<TError> Success { get; } = new(true, default!);

  /// <summary>
  /// Creates a <see cref="Result{TError}"/> with a failure status and the specified error value.
  /// </summary>
  /// <param name="error">The error produced by the operation.</param>
  public static Result<TError> Error(TError error) => new(false, error);

  /// <summary>
  /// Throws the specified exception if the operation failed.
  /// </summary>
  /// <param name="exception">The exception to throw.</param>
  /// <exception cref="Exception">The specified exception, if the operation failed.</exception>
  public void OrThrow(Exception exception)
  {
    if (!success) throw exception;
  }

  /// <summary>
  /// Determines whether the operation produced an error.
  /// </summary>
  /// <returns><c>true</c> if the operation produced an error; otherwise, <c>false</c>.</returns>
  public bool IsError() => IsError(out _);

  /// <summary>
  /// Determines whether the operation produced an error and gets the error value, if any.
  /// </summary>
  /// <param name="error">The error value produced by the operation, if any.</param>
  /// <returns><c>true</c> if the operation produced an error; otherwise, <c>false</c>.</returns>
  [SuppressMessage("ReSharper", "ParameterHidesMember")]
  public bool IsError([NotNullWhen(true)] out TError? error)
  {
    error = this.error;
    return !success;
  }
}