using System.Reflection;
using Ironclad.CodeAnalysis.Testing;
using Ironclad.ResultTypes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ResultTypes.CodeAnalysis.Tests;

/// <summary>
/// An analyzer test referencing the <see cref="ResultTypes"/> namespace in the test environment. (Tested code can
/// reference ResultTypes)
/// </summary>
/// <typeparam name="TAnalyzer">The type of analyzer to test.</typeparam>
internal sealed class ResultTypesAnalyzerTest<TAnalyzer> : IroncladAnalyzerTest<TAnalyzer> 
  where TAnalyzer : DiagnosticAnalyzer, new()
{
  public ResultTypesAnalyzerTest(params Assembly[] dependencies) : base(dependencies, typeof(Result<>).Assembly) { }
}