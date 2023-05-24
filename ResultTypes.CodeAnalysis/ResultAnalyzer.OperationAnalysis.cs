using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ResultTypes.CodeAnalysis;

internal sealed partial class ResultAnalyzer
{
	private static void AnalyzeNonBranchingOperation(IOperation operation,
		ref Dictionary<ISymbol, ResultTypeInstanceState> state,
		OperationBlockAnalysisContext context)
	{
		RegisterDiagnosticsForNonBranchingOperation(operation, state, context);
		ApplyStateChangesForNonBranchingOperation(operation, ref state);
		foreach (var childOperation in operation.ChildOperations)
			AnalyzeNonBranchingOperation(childOperation, ref state, context);
	}

	private static void RegisterDiagnosticsForNonBranchingOperation(
		IOperation operation,
		IReadOnlyDictionary<ISymbol, ResultTypeInstanceState> state,
		OperationBlockAnalysisContext context)
	{
		if (operation.IsOperationOnResultTypeInstance(out var operationDescriptor, out var instanceOperation)
		    && DiagnosticMap.TryGetValue(
			    (operationDescriptor, GetStateOfOperation(instanceOperation, state)),
			    out var diagnosticDescriptor)
		   )
		{
			context.ReportDiagnostic(Diagnostic.Create(
				diagnosticDescriptor,
				operation.Syntax.GetLocation(),
				instanceOperation.Syntax));
		}
	}

	private static void ApplyStateChangesForNonBranchingOperation(IOperation operation,
		ref Dictionary<ISymbol, ResultTypeInstanceState> state)
	{
		if (!IsChangingReferenceSymbolState(operation, state, out var symbol, out var newState))
			return;
		if (newState is ResultTypeInstanceState.Unknown)
			state.Remove(symbol);
		else
			state[symbol] = newState;
	}

	private static bool IsChangingReferenceSymbolState(IOperation operation,
		IReadOnlyDictionary<ISymbol, ResultTypeInstanceState> state,
		[NotNullWhen(true)] out ISymbol? symbolOperation,
		out ResultTypeInstanceState symbolState)
	{
		if (operation.IsAssignmentToResultTypeReferenceSymbol(out symbolOperation, out var valueOperation))
		{
			symbolState = GetStateOfOperation(valueOperation, state);
			return true;
		}

		if (operation.IsConversionOfResultTypeReferenceSymbolToValueType(out symbolOperation))
		{
			symbolState = ResultTypeInstanceState.Success;
			return true;
		}

		if (operation.IsOperationOnResultTypeInstance(out var operationIdentifier, out var instanceOperation)
		    && instanceOperation.IsSymbolReference(out symbolOperation)
		    && operationIdentifier is ResultTypeOperationIdentifier.OrThrow)
		{
			symbolState = ResultTypeInstanceState.Success;
			return true;
		}

		symbolOperation = null;
		symbolState = default;
		return false;
	}

	// TODO: lookup table
	private static ResultTypeInstanceState GetStateOfOperation(IOperation operation,
		IReadOnlyDictionary<ISymbol, ResultTypeInstanceState> state)
	{
		if (operation.IsSymbolReference(out var symbol) && state.TryGetValue(symbol, out var value))
			return value;

		if (operation is IConversionOperation
		    {
			    Operand.Type: { } operandType,
			    Type: { } conversionType,
		    } && conversionType.IsResultType(out var resultTypeSymbol))
		{
			if (operandType.Equals(resultTypeSymbol.ErrorType, SymbolEqualityComparer.Default))
				return ResultTypeInstanceState.Failure;
			if (operandType.Equals(resultTypeSymbol.ValueType, SymbolEqualityComparer.Default))
				return ResultTypeInstanceState.Success;
		}

		if (operation.IsOperationOnResultType(out var operationIdentifier))
		{
			switch (operationIdentifier)
			{
				case ResultTypeOperationIdentifier.SuccessConstructor:
					return ResultTypeInstanceState.Success;
				case ResultTypeOperationIdentifier.ErrorConstructor:
					return ResultTypeInstanceState.Failure;
			}
		}

		return ResultTypeInstanceState.Unknown;
	}
}