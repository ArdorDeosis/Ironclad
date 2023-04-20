using System.Diagnostics.CodeAnalysis;

namespace Ironclad.ResultTypes;

public interface IResult
{
  bool IsSuccess { get; }
  bool IsError { get; }
}

public interface IResult<TValue, TError> : IResult
{
  TValue Value { get; }
  TError Error { get; }
}

public class Success : IResult
{
  public bool IsSuccess => true;
  public bool IsError => false;
}

public interface ISuccess<T> : IResult
{
  public T Value { get; init; }
}

public class Failure : IResult
{
  public bool IsSuccess => false;
  public bool IsError => true;
}

public class Failure<T> : Failure
{
  public T Error { get; init; }
}




public class NullableResult<TValue, TError> 
  where TValue : notnull 
  where TError : notnull
{
  public TValue? Value { get; private init; }

  public TError? Error { get; private init; }

  [MemberNotNullWhen(true, nameof(Value))]
  [MemberNotNullWhen(false, nameof(Error))]
  public bool IsSuccess { get; private init; }

  [MemberNotNullWhen(false, nameof(Value))]
  [MemberNotNullWhen(true, nameof(Error))]
  public bool IsError => !IsSuccess;

  private NullableResult() { }

  // public static implicit operator Result<T>(string errorMessage) => Error(errorMessage);

  // public static implicit operator Result<T>(T value) => Success(value);

  // public static implicit operator Result(Result<T> result) => result.IsSuccess
  //   ? Result.Success
  //   : Result.Error(result.ErrorMessage);
    
  public static NullableResult<TValue, TError> Success(TValue value) => new() { Value = value, IsSuccess = true };

  public static NullableResult<TValue, TError> Failure(TError error) => new() { Error = error, IsSuccess = false };
}