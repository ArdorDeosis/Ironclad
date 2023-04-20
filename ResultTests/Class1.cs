using System.Diagnostics.CodeAnalysis;

namespace ResultTests;

public class Result<TValue, TError>
{
  private readonly bool success;
  private readonly TValue value;
  private readonly TError error;

  public TValue Value
  {
    get => success ? value : throw new Exception("Result is not success");
    private init => this.value = value;
  } 

  public static implicit operator TValue(Result<TValue, TError> result) => result.Value;
  
  public TValue? OrDefault() => success ? value : default;
  public TValue Or(TValue fallback) => success ? value : fallback;
  public TValue OrThrow(Exception exception) => success ? value : throw exception;

  public bool IsError([NotNullWhen(true)]out TError? error)
  {
    error = this.error;
    return success;
  }
}