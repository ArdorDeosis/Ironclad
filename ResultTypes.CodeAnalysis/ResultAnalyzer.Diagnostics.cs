using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ResultTypes.CodeAnalysis;

internal sealed partial class ResultAnalyzer
{
	/// <summary>
	/// A lookup table mapping from operations on result types and the state of the instance the operation is performed on
	/// to a <see cref="DiagnosticDescriptor"/>.
	/// </summary>
	private static readonly
		IReadOnlyDictionary<(ResultTypeOperationIdentifier operationDescriptor, ResultTypeInstanceState instanceState), DiagnosticDescriptor>
		DiagnosticMap =
			new Dictionary<(ResultTypeOperationIdentifier operationDescriptor, ResultTypeInstanceState instanceState), DiagnosticDescriptor>
			{
				{ (ResultTypeOperationIdentifier.Conversion, ResultTypeInstanceState.Unknown), Diagnostics.Conversion.OfUnknown },
				{ (ResultTypeOperationIdentifier.Conversion, ResultTypeInstanceState.Failure), Diagnostics.Conversion.OfError },
				{ (ResultTypeOperationIdentifier.OrFallback, ResultTypeInstanceState.Success), Diagnostics.Fallback.OnSuccess },
				{ (ResultTypeOperationIdentifier.OrFallback, ResultTypeInstanceState.Failure), Diagnostics.Fallback.OnFailure },
				{ (ResultTypeOperationIdentifier.OrThrow, ResultTypeInstanceState.Success), Diagnostics.Fallback.OnSuccess },
				{ (ResultTypeOperationIdentifier.OrThrow, ResultTypeInstanceState.Failure), Diagnostics.Fallback.AlwaysThrows },
				{ (ResultTypeOperationIdentifier.IsError, ResultTypeInstanceState.Success), Diagnostics.ExpressionIsAlways.False.OnSuccess },
				{ (ResultTypeOperationIdentifier.IsError, ResultTypeInstanceState.Failure), Diagnostics.ExpressionIsAlways.True.OnFailure },
				{ (ResultTypeOperationIdentifier.IsSuccess, ResultTypeInstanceState.Success), Diagnostics.ExpressionIsAlways.True.OnSuccess },
				{ (ResultTypeOperationIdentifier.IsSuccess, ResultTypeInstanceState.Failure), Diagnostics.ExpressionIsAlways.False.OnFailure },
			};
	
	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.CreateRange(DiagnosticMap.Values);
	
	/// <summary>
	/// Static data class for diagnostic descriptors.
	/// </summary>
	internal static class Diagnostics
	{
		private const string Category = "Ironclad.ResultTypes";

		// Diagnostic Numbering:
		// 0000-0009: will throw
		// 0010-0019: might throw
		// 0020-0029: redundant operation

		internal static class Conversion
		{
			internal static readonly DiagnosticDescriptor OfError = new("IC0001",
				"Conversion of failed Result type",
				"Conversion always throws an exception, since {0} is a failure.",
				Category,
				DiagnosticSeverity.Error, 
				true,
				"This diagnostic is triggered when a failed Result type is converted, which always throws an exception.");

			internal static readonly DiagnosticDescriptor OfUnknown = new("IC0010",
				"Conversion of unknown Result type",
				"Conversion of {0} might throw an exception if {0} is a failure.",
				Category,
				DiagnosticSeverity.Warning, 
				true,
				"This diagnostic is triggered when an unknown Result type is converted, which might throw an exception if the type is a failure.");
		}

		internal static class Fallback
		{
			internal static readonly DiagnosticDescriptor AlwaysThrows = new("IC0002",
				"Result fallback always throws",
				"Result fallback always throws, since {0} is a failure.",
				Category,
				DiagnosticSeverity.Warning, 
				true,
				"This diagnostic is triggered when a result fallback value that always throws an exception is detected, which is the fallback value for a failed Result.");

			internal static readonly DiagnosticDescriptor OnSuccess = new("IC00020",
				"Redundant result fallback on successful Result type",
				"Result fallback is redundant, since {0} is a success.",
				Category,
				DiagnosticSeverity.Warning, 
				true,
				"This diagnostic is triggered when a result fallback on a successful Result is detected, which is redundant.");

			internal static readonly DiagnosticDescriptor OnFailure = new("IC0021",
				"Result fallback on failed Result type is always chosen",
				"Result fallback is always chosen, since {0} is a failure.",
				Category,
				DiagnosticSeverity.Warning, 
				true,
				"This diagnostic is triggered when a result fallback on a failed Result is detected, which is always chosen.");
		}

		internal static class ExpressionIsAlways
		{
			private static DiagnosticDescriptor GetDescriptor(bool expressionValue, bool success) => new("IC0022",
				$"Expression is always {(expressionValue ? "true" : "false")}",
				$"This expression is always {(expressionValue ? "true" : "false")} because {{0}} is a {(success ? "success" : "failure")}.",
				Category,
				DiagnosticSeverity.Warning, 
				true,
				"The diagnostic is triggered because the expression checks the success state of a Result object, but the state of the Result (success or failure) is already known at compile time. This makes the expression always evaluate to the same value, which could indicate a potential error in the code logic.");  
			
			internal static class True
			{
				internal static readonly DiagnosticDescriptor OnSuccess = GetDescriptor(true, true);
				internal static readonly DiagnosticDescriptor OnFailure = GetDescriptor(true, false);
			}
			
			internal static class False
			{
				internal static readonly DiagnosticDescriptor OnSuccess = GetDescriptor(false, true);
				internal static readonly DiagnosticDescriptor OnFailure = GetDescriptor(false, false);
			}
		}
	}
}