namespace Ironclad.ResultTypes.Tests;

[TestFixture]
public class ResultWithoutValueTests
{
  private const int ErrorValue = 0xBEEF;
  private static readonly Result<int> ErrorResult = Result<int>.Error(ErrorValue);

  [Test]
  public void ImplicitConversionToBool_ReturnsTrue_WhenSuccess()
  {
    // ACT
    bool result = Result<int>.Success;

    // ASSERT
    Assert.That(result, Is.True);
  }

  [Test]
  public void ImplicitConversionToBool_ReturnsFalse_WhenError()
  {
    // ACT
    bool result = ErrorResult;

    // ASSERT
    Assert.That(result, Is.False);
  }

  [Test]
  public void ImplicitConversionFromError_CreatesResultWithFailureStatus()
  {
    // ACT
    Result<int> result = ErrorValue;

    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(result.IsError());
      Assert.That(result.IsError(out _));
    });
  }

  [Test]
  public void ImplicitConversionFromError_CreatesResultWithErrorValue()
  {
    // ACT
    Result<int> result = ErrorValue;
    result.IsError(out var value);

    // ASSERT
    Assert.That(value, Is.EqualTo(ErrorValue));
  }

  [Test]
  public void CreatedError_HasCorrectValue()
  {
    // ACT
    var result = Result<int>.Error(ErrorValue);
    result.IsError(out var value);

    // ASSERT
    Assert.That(value, Is.EqualTo(ErrorValue));
  }

  [Test]
  public void OrThrow_ThrowsException_WhenResultIsError()
  {
    // ACT & ASSERT
    Assert.That(() => ErrorResult.OrThrow(new TestException()), Throws.InstanceOf<TestException>());
  }

  [Test]
  public void OrThrow_DoesNotThrowException_WhenResultIsSuccess()
  {
    // ACT & ASSERT
    Assert.That(() => Result<int>.Success.OrThrow(new TestException()), Throws.Nothing);
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