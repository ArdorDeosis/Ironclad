namespace Ironclad.ResultTypes.Tests;

[TestFixture]
public class ResultWithValueTests
{
  private const string ErrorValue = "😱";
  private const int SuccessValue = 0xC0FFEE;
  private const int FallbackValue = 0xBEEF;

  private static readonly Result<int, string> SuccessResult = SuccessValue;
  private static readonly Result<int, string> ErrorResult = ErrorValue;

  [Test]
  public void ImplicitConversion_SuccessfulResult_ReturnsValue()
  {
    // ACT
    int value = SuccessResult;

    // ASSERT
    Assert.That(value, Is.EqualTo(SuccessValue));
  }

  [Test]
  public void ImplicitConversion_FailedResult_ThrowsInvalidOperationException()
  {
    // ACT & ASSERT
    Assert.That(() =>
    {
      int _ = ErrorResult;
    }, Throws.InvalidOperationException);
  }

  [Test]
  public void ImplicitConversion_Value_SuccessfulResult()
  {
    // ACT
    Result<int, string> result = SuccessValue;
    int value = result;

    // ASSERT
    Assert.That(value, Is.EqualTo(SuccessValue));
  }

  [Test]
  public void ImplicitConversion_Error_FailedResult()
  {
    // ACT
    Result<int, string> result = ErrorValue;

    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(result.IsError());
      Assert.That(result.IsError(out _));
    });
  }
  
  [Test]
  public void CreateSuccess_HasCorrectValue()
  {
    // ACT
    var result = Result<int, string>.Success(SuccessValue);
    var value = result.OrThrow(new Exception());

    // ASSERT
    Assert.That(value, Is.EqualTo(SuccessValue));
  }

  [Test]
  public void CreateError_HasCorrectValue()
  {
    // ACT
    var result = Result<int, string>.Error(ErrorValue);
    

    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(result.IsError(out var value));
      Assert.That(value, Is.EqualTo(ErrorValue));
    });
  }

  [Test]
  public void Or_SuccessfulResult_ReturnsValue()
  {
    // ACT
    var value = SuccessResult.Or(FallbackValue);

    // ASSERT
    Assert.That(value, Is.EqualTo(SuccessValue));
  }

  [Test]
  public void Or_FailedResult_ReturnsFallback()
  {
    // ACT
    var value = ErrorResult.Or(FallbackValue);

    // ASSERT
    Assert.That(value, Is.EqualTo(FallbackValue));
  }

  [Test]
  public void OrDefault_SuccessfulResult_ReturnsValue()
  {
    // ACT
    var value = SuccessResult.OrDefault;

    // ASSERT
    Assert.That(value, Is.EqualTo(SuccessValue));
  }

  [Test]
  public void OrDefault_FailedResult_ReturnsDefaultValue()
  {
    // ACT
    var value = ErrorResult.OrDefault;

    // ASSERT
    Assert.That(value, Is.EqualTo(default(int)));
  }

  [Test]
  public void OrThrow_SuccessfulResult_ReturnsValue()
  {
    // ACT & ASSERT
    Assert.That(SuccessResult.OrThrow(new TestException()), Is.EqualTo(SuccessValue));
  }

  [Test]
  public void OrThrow_FailedResult_ThrowsException()
  {
    // ACT & ASSERT
    Assert.That(() => { ErrorResult.OrThrow(new TestException()); }, Throws.InstanceOf<TestException>());
  }
  

  [Test]
  public void IsError_ReturnsFalse_WhenResultIsSuccess()
  {
    // ACT & ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(Result<int>.Success.IsError(), Is.False);
      Assert.That(Result<int>.Success.IsError(out _), Is.False);
    });
  }

  [Test]
  public void IsError_ReturnsTrue_WhenResultIsError()
  {
    // ACT & ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(ErrorResult.IsError(), Is.True);
      Assert.That(ErrorResult.IsError(out _), Is.True);
    });
  }

  [Test]
  public void IsError_OutParameterIsErrorValue_WhenResultIsError()
  {
    // ACT
    ErrorResult.IsError(out var error);

    // ASSERT
    Assert.That(error, Is.EqualTo(ErrorValue));
  }
}