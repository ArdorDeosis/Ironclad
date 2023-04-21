namespace Ironclad.ResultTypes.Tests;

[TestFixture]
public class ResultWithoutValueTests
{
  private const int ErrorValue = 0xBEEF;

  [Test]
  public void ImplicitConversionToBool_ReturnsTrue_WhenSuccess()
  {
    // ARRANGE
    bool result = Result<int>.Success;

    // ASSERT
    Assert.That(result, Is.True);
  }

  [Test]
  public void ImplicitConversionToBool_ReturnsFalse_WhenError()
  {
    // ARRANGE
    var errorResult = Result<int>.Error(ErrorValue);

    // ACT
    bool result = errorResult;

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
  public void OrThrow_ThrowsException_WhenResultIsError()
  {
    // ARRANGE
    var errorResult = Result<int>.Error(ErrorValue);
    var exception = new TestException();

    // ASSERT
    Assert.That(() => errorResult.OrThrow(exception), Throws.InstanceOf<TestException>());
  }

  [Test]
  public void OrThrow_DoesNotThrowException_WhenResultIsSuccess()
  {
    // ARRANGE
    var successResult = Result<int>.Success;
    var exception = new TestException();

    // ASSERT
    Assert.DoesNotThrow(() => successResult.OrThrow(exception));
  }

  [Test]
  public void IsError_ReturnsFalse_WhenResultIsSuccess()
  {
    // ARRANGE
    var successResult = Result<int>.Success;

    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(successResult.IsError(), Is.False);
      Assert.That(successResult.IsError(out _), Is.False);
    });
  }

  [Test]
  public void IsError_ReturnsTrue_WhenResultIsError()
  {
    // ARRANGE
    var errorResult = Result<int>.Error(ErrorValue);

    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(errorResult.IsError(), Is.True);
      Assert.That(errorResult.IsError(out _), Is.True);
    });
  }

  [Test]
  public void IsError_OutParameterIsErrorValue_WhenResultIsError()
  {
    // ARRANGE
    var errorResult = Result<int>.Error(ErrorValue);

    // ACT
    errorResult.IsError(out var error);

    // ASSERT
    Assert.That(error, Is.EqualTo(ErrorValue));
  }

  private class TestException : Exception { }
}