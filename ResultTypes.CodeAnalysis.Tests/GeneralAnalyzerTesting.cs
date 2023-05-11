using System.Collections.Immutable;
using Ironclad.ResultTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace ResultTypes.CodeAnalysis.Tests;

internal sealed class GeneralAnalyzerTesting
{
  [Test]
  public async Task SimpleAnalyzerTest()
  {
    var x = Result<int, string>.Success(3);
    var testCode = @"
using System;
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
    
    
    var test = new CSharpAnalyzerTest<ResultAnalyzer, NUnitVerifier>
    {
      TestCode = testCode,
      ReferenceAssemblies = ReferenceAssemblies.Default
        .WithAssemblies(ImmutableArray.Create<string>("Ironclad.ResultTypes")),
    };

    var expectedDiagnostic = new DiagnosticResult("MyCustomAnalyzer", DiagnosticSeverity.Warning)
      .WithLocation(7, 9)
      .WithArguments("TestMethod");

    await test.RunAsync();
  }
}