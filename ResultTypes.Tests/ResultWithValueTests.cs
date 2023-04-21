namespace Ironclad.ResultTypes.Tests;

[TestFixture]
public class ResultWithValueTests
{
  private const string ErrorValue = "oops";
  private const int SuccessValue = 0xC0FFEE;
  
  [Test]
  public void ImplicitConversion_SuccessfulResult_ReturnsValue()
  {
    // ARRANGE
    Result<int, string> result = SuccessValue;
    
    // ACT
    int value = result;
    
    // ASSERT
    Assert.That(value, Is.EqualTo(42));
  }

  [Test]
  public void ImplicitConversion_FailedResult_ThrowsInvalidOperationException()
  {
    // ARRANGE
    Result<int, string> result = ErrorValue;
    Assert.Throws<InvalidOperationException>(() => { });
  }

  [Test]
  public void ImplicitConversion_Value_SuccessfulResult()
  {
    Result<int, string> result = 42;
    Assert.IsTrue(result.IsSuccess());
    Assert.AreEqual(42, result);
  }

  [Test]
  public void ImplicitConversion_Error_FailedResult()
  {
    Result<int, string> result = "oops";
    Assert.IsTrue(result.IsError());
    Assert.AreEqual("oops", result.Error);
  }

  [Test]
  public void Or_SuccessfulResult_ReturnsValue()
  {
    Result<int, string> result = Result.Success(42);
    int value = result.Or(0);
    Assert.AreEqual(42, value);
  }

  [Test]
  public void Or_FailedResult_ReturnsFallback()
  {
    Result<int, string> result = Result.Error("oops");
    int value = result.Or(0);
    Assert.AreEqual(0, value);
  }

  [Test]
  public void OrDefault_SuccessfulResult_ReturnsValue()
  {
    Result<int, string> result = Result.Success(42);
    int? value = result.OrDefault;
    Assert.AreEqual(42, value);
  }

  [Test]
  public void OrDefault_FailedResult_ReturnsDefaultValue()
  {
    Result<int, string> result = Result.Error("oops");
    int? value = result.OrDefault;
    Assert.IsNull(value);
  }

  [Test]
  public void OrThrow_SuccessfulResult_ReturnsValue()
  {
    Result<int, string> result = Result.Success(42);
    int value = result.OrThrow(new AssertionException("Should not throw"));
    Assert.AreEqual(42, value);
  }

  [Test]
  public void OrThrow_FailedResult_ThrowsException()
  {
    Result<int, string> result = Result.Error("oops");
    Assert.Throws<AssertionException>(() =>
    {
      int value = result.OrThrow(new AssertionException("Expected exception"));
    });
  }
}