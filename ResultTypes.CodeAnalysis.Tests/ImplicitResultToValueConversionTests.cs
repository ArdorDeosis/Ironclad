using Microsoft.CodeAnalysis.Testing;
using ResultTypes.CodeAnalysis.Tests.TestHelpers;

namespace ResultTypes.CodeAnalysis.Tests;

internal sealed class ImplicitResultToValueConversionTests
{
  [Test]
  public async Task SimpleAnalyzerTest()
  {
    const string testCode = @"
using System;
using Ironclad.ResultTypes;

class TestClass
{
    private readonly Result<int, string> resultField;

    private Result<int, string> MethodReturningResult() => throw new NotImplementedException();

    private void TestMethod(Result<int, string> resultParameter)
    {
        var resultLocal = MethodReturningResult();

        int fromField = resultField;
        int fromParameter = resultParameter;
        int fromLocal = resultLocal;

        int fromField2 = resultField;
        int fromParameter2 = resultParameter;
        int fromLocal2 = resultLocal;
    }
}";

    var test = new ResultTypesAnalyzerTest<ResultAnalyzer> { 
      TestCode = testCode, 
      ExpectedDiagnostics =
      {
        new DiagnosticResult(ResultAnalyzer.Diagnostics.Conversion.OfUnknown)
          .WithLocation(15, 25)
          .WithArguments("resultField"),
        new DiagnosticResult(ResultAnalyzer.Diagnostics.Conversion.OfUnknown)
          .WithLocation(16, 29)
          .WithArguments("resultParameter"),
        new DiagnosticResult(ResultAnalyzer.Diagnostics.Conversion.OfUnknown)
          .WithLocation(17, 25)
          .WithArguments("resultLocal"),
      },
    };


    await test.RunAsync();
  }
  
  [Test]
  public async Task DebugAnalyzer()
  {
    const string testCode = @"
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

    if (value1 > 3)
    {
      
    }
    
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
}";

    var test = new ResultTypesAnalyzerTest<ResultAnalyzer> { 
      TestCode = testCode,
    };


    await test.RunAsync();
  }
}