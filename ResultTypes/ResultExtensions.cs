namespace Ironclad.ResultTypes;

/// <summary>
/// Provides extension methods for working with <see cref="Result{TValue, TError}"/> and <see cref="Result{TError}"/>
/// objects.
/// </summary>
public static class ResultExtensions
{
  /// <summary>
  /// Maps the value and error of a <see cref="Result{TValueOriginal, TErrorOriginal}"/> object to a new
  /// <see cref="Result{TValueNew, TErrorNew}"/> object using the specified mapping functions.
  /// </summary>
  /// <typeparam name="TValueOriginal">The type of the value in the original result.</typeparam>
  /// <typeparam name="TValueNew">The type of the value in the new result.</typeparam>
  /// <typeparam name="TErrorOriginal">The type of the error in the original result.</typeparam>
  /// <typeparam name="TErrorNew">The type of the error in the new result.</typeparam>
  /// <param name="result">The original result to map.</param>
  /// <param name="mapValue">The function to map the value of the original result to the new result.</param>
  /// <param name="mapError">The function to map the error of the original result to the new result.</param>
  /// <returns>A new <see cref="Result{TValueNew, TErrorNew}"/> object with the mapped value or error.</returns>
  public static Result<TValueNew, TErrorNew> Map<TValueOriginal, TValueNew, TErrorOriginal, TErrorNew>(
    Result<TValueOriginal, TErrorOriginal> result,
    Func<TValueOriginal, TValueNew> mapValue,
    Func<TErrorOriginal, TErrorNew> mapError) =>
    result.IsError(out var error) ? mapError(error) : mapValue(result);

  /// <summary>
  /// Maps the value of a <see cref="Result{TValueOriginal, TError}"/> object to a new
  /// <see cref="Result{TValueNew, TError}"/> object using the specified mapping function.
  /// </summary>
  /// <typeparam name="TValueOriginal">The type of the value in the original result.</typeparam>
  /// <typeparam name="TValueNew">The type of the value in the new result.</typeparam>
  /// <typeparam name="TError">The type of the error in the original and new result.</typeparam>
  /// <param name="result">The original result to map.</param>
  /// <param name="mapValue">The function to map the value of the original result to the new result.</param>
  /// <returns>A new <see cref="Result{TValueNew, TError}"/> object with the mapped value or original error.</returns>
  public static Result<TValueNew, TError> MapValue<TValueOriginal, TValueNew, TError>(
    Result<TValueOriginal, TError> result,
    Func<TValueOriginal, TValueNew> mapValue) =>
    result.IsError(out var error) ? error : mapValue(result);

  /// <summary>
  /// Maps the error of a <see cref="Result{TValue, TErrorOriginal}"/> object to a new
  /// <see cref="Result{TValue, TErrorNew}"/> object using the specified mapping function.
  /// </summary>
  /// <typeparam name="TValue">The type of the value in the original and new result.</typeparam>
  /// <typeparam name="TErrorOriginal">The type of the error in the input result.</typeparam>
  /// <typeparam name="TErrorNew">The type of the error in the output result.</typeparam>
  /// <param name="result">The input result.</param>
  /// <param name="mapError">The mapping function for the error.</param>
  /// <returns>A new <see cref="Result{TValue, TErrorNew}"/> object with the mapped error or original value.</returns>
  public static Result<TValue, TErrorNew> MapError<TValue, TErrorOriginal, TErrorNew>(
    Result<TValue, TErrorOriginal> result,
    Func<TErrorOriginal, TErrorNew> mapError) =>
    result.IsError(out var error) ? mapError(error) : Result<TValue, TErrorNew>.Success(result);
  
  /// <summary>
  /// Maps the error of a <see cref="Result{TErrorOriginal}"/> object to a new <see cref="Result{TErrorNew}"/> object
  /// using the specified mapping function.
  /// </summary>
  /// <typeparam name="TErrorOriginal">The type of the error in the input result.</typeparam>
  /// <typeparam name="TErrorNew">The type of the error in the output result.</typeparam>
  /// <param name="result">The input result.</param>
  /// <param name="mapError">The mapping function for the error.</param>
  /// <returns>
  /// A new <see cref="Result{TErrorNew}"/> object with the mapped error or <see cref="Result{TErrorNew}.Success"/>.
  /// </returns>
  public static Result<TErrorNew> MapError<TErrorOriginal, TErrorNew>(
    Result<TErrorOriginal> result,
    Func<TErrorOriginal, TErrorNew> mapError) =>
    result.IsError(out var error) ? mapError(error) : Result<TErrorNew>.Success;
}