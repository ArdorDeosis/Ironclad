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
    Assert.That(() => { SuccessResult.OrThrow(new TestException()); }, Throws.Nothing);
  }

  [Test]
  public void OrThrow_FailedResult_ThrowsException()
  {
    // ACT & ASSERT
    Assert.That(() => { ErrorResult.OrThrow(new TestException()); }, Throws.InstanceOf<TestException>());
  }
}