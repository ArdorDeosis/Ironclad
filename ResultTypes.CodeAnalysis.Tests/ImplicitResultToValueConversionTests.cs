using Microsoft.CodeAnalysis.Testing;

namespace ResultTypes.CodeAnalysis.Tests;

internal sealed class ImplicitResultToValueConversionTests
{
  [Test]
  public async Task SimpleAnalyzerTest()
  {
    const string testCode = @"
using Ironclad.ResultTypes;

class TestClass
{
    private readonly Result<int, string> resultField;
    void TestMethod(Result<int, string> resultParameter)
    {
        var resultLocal = Result<int, string>.Success(5);

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
        new DiagnosticResult(ResultAnalyzer.ImplicitConversionOfUnknown)
          .WithLocation(11, 25),
        new DiagnosticResult(ResultAnalyzer.ImplicitConversionOfUnknown)
          .WithLocation(12, 29),
        new DiagnosticResult(ResultAnalyzer.ImplicitConversionOfUnknown)
          .WithLocation(13, 25),
      },
    };


    await test.RunAsync();
  }
}