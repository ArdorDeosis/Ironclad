using Ironclad.ResultTypes;

namespace ResultTypes.Playground;

public class DummyData { public object Value { get; } }


public class Playground
{
  private readonly int? nullableValue = default!;
  private readonly DummyData? nullableReference = default!;
  
  private readonly Result<int, string> valueResult = null!;
  private readonly Result<int?, string> nullableValueResult = null!;
  private readonly Result<DummyData, string> referenceResult = null!;
  private readonly Result<DummyData?, string> nullableReferenceResult = null!;
  
  public void OrThrow_ValueNullabilityIsCorrect()
  {
    // should be of type int
    var value1 = valueResult.OrThrow(new Exception());
    
    // should be of type int?
    var value2 = nullableValueResult.OrThrow(new Exception());
    
    // should be of type DummyData
    var value3 = referenceResult.OrThrow(new Exception());
    
    // should be of type DummyData?
    var value4 = nullableReferenceResult.OrThrow(new Exception());
  }
  
  public void OrDefault_ValueNullabilityIsCorrect()
  {
    // should be of type int
    var value1 = valueResult.OrDefault;
    
    // should be of type int?
    var value2 = nullableValueResult.OrDefault;
    
    // both should be of type DummyData?
    var value3 = referenceResult.OrDefault;
    var value4 = nullableReferenceResult.OrDefault;
  }
  
  public void ValueResult_Or_ValueNullabilityIsCorrect()
  {
    // all should be of type int
    var value1 = valueResult.Or(5);
    
    // should not compile
    // var value2 = valueResult.Or(nullableValue);
    // var value3 = valueResult.Or(null);
  }
  
  public void NullableValueResult_Or_ValueNullabilityIsCorrect()
  {
    // all should be of type int?
    // no warnings
    var value1 = nullableValueResult.Or(5);
    var value2 = nullableValueResult.Or(nullableValue);
    var value3 = nullableValueResult.Or(null);
    
  }
  public void ReferenceResult_Or_ValueNullabilityIsCorrect()
  {
    // all should be of type DummyData
    // no warnings
    var value1 = referenceResult.Or(new DummyData());
    // warnings
    var value2 = referenceResult.Or(nullableReference);
    var value3 = referenceResult.Or(null);
  }
  
  public void NullableReferenceResult_Or_ValueNullabilityIsCorrect()
  {
    // all should be of type DummyData?
    var value1 = nullableReferenceResult.Or(new DummyData());
    var value2 = nullableReferenceResult.Or(nullableReference);
    var value3 = nullableReferenceResult.Or(null);
  }

  public void Test()
  {
    DummyData x = referenceResult;

  }
}