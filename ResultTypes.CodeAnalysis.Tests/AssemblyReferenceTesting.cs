using System.Collections.Immutable;
using Ironclad.ResultTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace ResultTypes.CodeAnalysis.Tests;

internal class MyTest : CSharpAnalyzerTest<ResultAnalyzer, NUnitVerifier>
{
  protected override ProjectState CreateProjectState(ProjectId projectId)
  {
    
  }
}

internal sealed class AssemblyReferenceTesting
{
  [Test]
  public async Task SimpleReferenceTest()
  {
    const string testCode = "using Ironclad.ResultTypes;";

    var test = new CSharpAnalyzerTest<ResultAnalyzer, NUnitVerifier>
    {
      TestCode = testCode,
      ReferenceAssemblies = ReferenceAssemblies.Default
        .AddAssemblies(ImmutableArray.Create("Ironclad.ResultTypes", "ResultTypes", typeof(Result<>).Assembly.Location)),
      TestState =
      {
        ReferenceAssemblies = ReferenceAssemblies.Default
          .AddAssemblies(ImmutableArray.Create("Ironclad.ResultTypes", "ResultTypes", typeof(Result<>).Assembly.Location)),

      },
    };
    test.SolutionTransforms.Add((solution, projectId) =>
    {
      return solution.AddMetadataReference(ProjectId.CreateNewId(),
        MetadataReference.CreateFromFile(typeof(Result<>).Assembly.Location));
      Console.WriteLine($"{solution.Workspace}");
      foreach (var project in solution.Projects)
      {
        Console.WriteLine($"{project.Name}");
        foreach (var projectReference in project.AllProjectReferences)
          Console.WriteLine($"    {string.Join(", ", projectReference.Aliases)}");
      }
      return solution;
    });

    await test.RunAsync();
  }
}