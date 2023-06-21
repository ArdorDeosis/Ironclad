using ResultTypes.CodeAnalysis.Tests.TestHelpers;

namespace ResultTypes.CodeAnalysis.Tests;

internal sealed class PlaygroundTesting
{
  [Test]
  public async Task SimpleAnalyzerTest()
  {
    const string testCode = @"
using System;
class TestClass
{
  private void AnotherTestMethod()
  {
		var x = 5;
		while (x > 0) {
			x--;
		}
		Console.WriteLine(x);
  }
}";

    var test = new ResultTypesAnalyzerTest<TestAnalyzer> { TestCode = testCode };
    await test.RunAsync();
  }
}