using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ResultTypes.CodeAnalysis;

internal sealed partial class ResultAnalyzer
{
  /// <inheritdoc />
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    ImmutableArray.Create(
      ConversionOfError,
      ConversionOfUnknown,
      RedundantFallback,
      FallbackIsAlwaysUsed,
      FallbackAlwaysThrows,
      ExpressionIsAlwaysTheSame
    );

  // TODO: descriptions
  
  public static readonly DiagnosticDescriptor ConversionOfError = new("IC001",
    "conversion of failed Result type",
    "conversion always throws an exception, since {0} is a failure", "Ironclad.ResultTypes",
    DiagnosticSeverity.Error, isEnabledByDefault: true, description: "description");

  public static readonly DiagnosticDescriptor ConversionOfUnknown = new("IC002",
    "conversion of unknown Result type",
    "conversion of {0} might throw an exception if {0} is a failure", "Ironclad.ResultTypes",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description");
  
  // code fix?
  public static readonly DiagnosticDescriptor RedundantFallback = new("IC003",
    "redundant fallback on result: {0} is a success",
    "", "Ironclad.ResultTypes",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description");
  
  // code fix
  public static readonly DiagnosticDescriptor FallbackIsAlwaysUsed = new("IC004",
    "expression is always {0}, since {1} is a failure",
    "", "Ironclad.ResultTypes",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description");
  
  public static readonly DiagnosticDescriptor FallbackAlwaysThrows = new("IC005",
    "fallback always throws, since {1} is a failure",
    "", "Ironclad.ResultTypes",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description");
  
  public static readonly DiagnosticDescriptor ExpressionIsAlwaysTheSame = new("IC006",
    "expression is always {0}; {1} is a {2}",
    "", "Ironclad.ResultTypes",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description");
}