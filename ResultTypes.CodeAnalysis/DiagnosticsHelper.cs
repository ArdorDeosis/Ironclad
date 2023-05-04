using Microsoft.CodeAnalysis;

namespace ResultTypes.CodeAnalysis;

internal enum ResultSymbol
{
  ImplicitConversion,
  OrThrow,
  OrDefault,
  Or,
  IsError,
  IsErrorOutValue,
}

internal static class DiagnosticsHelper
{
  public static DiagnosticDescriptor Found(ResultSymbol resultSymbol) =>
    new(
      "IC",
      "Title",
      $"found {resultSymbol}",
      "TESTING STUFF",
      DiagnosticSeverity.Warning,
      true,
      "description"
    );
}