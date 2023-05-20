using Ironclad.ResultTypes;

namespace ResultTypes.CodeAnalysis.Tests;

[TestFixture]
public class TestIdeas
{
  private readonly Result<int, string> result = null!;
  
  // NOTES:
  //
  // ways to determine the state of a result:
  // - conversion
  // - OrThrow()
  // - IsError(), IsError(out TError)
  // - assigning .Success or .Error
  // - assigning via implicit conversion
  //
  // invalid operations in error state:
  // - conversion in error state
  // - using out error value in success state
  // redundant operations in any state:
  // - Or(), OrDefault(), OrThrow()
  // - IsError(), IsError(out TError)
  
  [Test]
  public void Conversion_NoCheck_ShouldWarnAboutMissingCheck()
  {
    var value1 = (int)Result<int, string>.Success(6);
  }
  
  [Test]
  public void ImplicitConversion_NoCheck_ShouldWarnAboutMissingCheck()
  {
    var value1 = 0 + Result<int, string>.Success(6);
    var value2 = 0 + (Result<int, string>)6;
    var value3 = 0 + result;
    object obj = result;
  }
  
  [Test]
  public void ImplicitConversion_UsingOr_ShouldNotWarnAboutMissingCheck()
  {
    var value1 = 0 + result.OrDefault;
    var value2 = 0 + result.Or(default);
  }
  
  [Test]
  public void ImplicitConversion_InsideCheck_ShouldNotWarnAboutMissingCheck()
  {
    // TODO: use error returning value, too
    if (result.IsError())
    {
      var value = 0 + result;
    }
  }
  
  [Test]
  public void ImplicitConversion_AfterEarlyReturn_ShouldNotWarnAboutMissingCheck()
  {
    // TODO: test throw, too
    if (result.IsError())
      return;
    var value = 0 + result;
  }
  
  [Test]
  public void UsingErrorValue_AfterEarlyReturn_ShouldWarnAboutInvalidErrorValue()
  {
    var local = Result<int, int>.Success(432);
    if (local.IsError(out var error))
      return;
    var value = "" + error;
  }
  
  [Test]
  public void UsingErrorValue_InsideSuccessCase_ShouldWarnAboutInvalidErrorValue()
  {
    if (!result.IsError(out var error))
    {
      var value = "" + error;
    }
  }
  
  [Test]
  public void CheckingForError_AfterOrThrow_ShouldWarnAboutObsoleteCheck()
  {
    var value = result.OrThrow(new Exception());
    // TODO: also check IsError without out parameter and negated version
    if (result.IsError(out var error))
    {
      
    }
  }
  
  [Test]
  public void UsingAnyOr_AfterOrThrow_ShouldWarnAboutObsoleteCall()
  {
    var value = result.OrThrow(new Exception());
    
    // should all warn about obsolete call
    value = result.Or(0);
    value = result.OrDefault;
    value = result.OrThrow(default!);
  }
  
  [Test]
  public void UsingAnyOr_AfterErrorCheck_ShouldWarnAboutObsoleteCall()
  {
    var value = 0;
    
    // both cases should warn (e.g. 'fallback not needed' vs. 'always fallback')
    if(result.IsError())
    {
      value = result.Or(0);
      value = result.OrDefault;
      value = result.OrThrow(default!);
    }
    else
    {
      value = result.Or(0);
      value = result.OrDefault;
      value = result.OrThrow(default!);
    }

    if (result.IsError())
    {
      var x = 4;
    }
  }
  
  // TODO: check that reassignments reset the analysis state
}