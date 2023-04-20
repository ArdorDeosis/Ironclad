using System.Diagnostics.CodeAnalysis;

namespace TestingResultTypes;

public class Result<TValue, TError>
{
  private readonly bool success;
  private readonly TValue value;
  private readonly TError error;

  private Result(bool success, TValue value, TError error)
  {
    this.success = success;
    this.value = value;
    this.error = error;
  }

  public static implicit operator TValue(Result<TValue, TError> result) => result.success 
    ? result.value 
    : throw new Exception("Result is not success");
  
  public static implicit operator Result<TValue, TError>(TValue value) => Success(value);
  
  public static implicit operator Result<TValue, TError>(TError error) => Error(error);
  
  public static Result<TValue, TError> Success(TValue value) => new(true, value, default!);
  
  public static Result<TValue, TError> Error(TError error) => new(false, default!, error);

  public TValue Or(TValue fallback) => success ? value : fallback;
  public TValue? OrDefault => success ? value : default;
  public TValue OrThrow(Exception exception) => success ? value : throw exception;

  public bool IsError() => IsError(out _);

  [SuppressMessage("ReSharper", "ParameterHidesMember")]
  public bool IsError([NotNullWhen(true)] out TError? error)
  {
    error = this.error;
    return !success;
  }
}

public class Result<TError>
{
  private readonly bool success;
  private readonly TError error;

  private Result(bool success, TError error)
  {
    this.success = success;
    this.error = error;
  }

  public static implicit operator bool(Result<TError> result) => result.success;
  public static implicit operator Result<TError>(TError error) => new(false, error);
  
  public static Result<TError> Success { get; } = new(true, default!);
  
  public static Result<TError> Error(TError error) => new(false, error);

  public void OrThrow(Exception exception)
  {
    if (!success) throw exception;
  }

  public bool IsError() => IsError(out _);

  [SuppressMessage("ReSharper", "ParameterHidesMember")]
  public bool IsError([NotNullWhen(true)] out TError? error)
  {
    error = this.error;
    return !success;
  }
}