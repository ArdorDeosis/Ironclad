using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Model;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Ironclad.CodeAnalysis.Testing;

/// <summary>
/// A <see cref="CSharpAnalyzerTest{TAnalyzer,TVerifier}"/> using an NUnit verifier and offering the possibility to add
/// assembly references to the environment in which the analyzer is tested.
/// </summary>
/// <typeparam name="TAnalyzer">The type of analyzer to test.</typeparam>
public class IroncladAnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, NUnitVerifier>
  where TAnalyzer : DiagnosticAnalyzer, new()
{
  private readonly Assembly[] dependencies;

  /// <param name="dependencies">Assemblies to be referenced by the generated test project.</param>
  public IroncladAnalyzerTest(params Assembly[] dependencies) => this.dependencies = dependencies;
  
  /// <summary>
  /// Convenience constructor to make this callable with a collection and single assemblies in one call.
  /// </summary>
  /// <example>
  /// <code>
  /// new IroncladAnalyzerTest(listOfAssemblies, singleAssembly1, singleAssembly2);
  /// </code>
  /// </example>
  public IroncladAnalyzerTest(IEnumerable<Assembly> dependencies, params Assembly[] furtherDependencies) => 
    this.dependencies = dependencies.Concat(furtherDependencies).ToArray();

  /// <inheritdoc />
  protected override async Task<Project> CreateProjectImplAsync(
    EvaluatedProjectState primaryProject,
    ImmutableArray<EvaluatedProjectState> additionalProjects,
    CancellationToken cancellationToken)
  {
    var projectState = await base.CreateProjectImplAsync(primaryProject, additionalProjects, cancellationToken);
    return projectState.AddMetadataReferences(dependencies.Select(assembly =>
      MetadataReference.CreateFromFile(assembly.Location)));
  }
}