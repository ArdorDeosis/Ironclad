using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ResultTypes.CodeAnalysis;

internal sealed partial class ResultAnalyzer
{
  private static void AnalyzeNonBranchingOperation(IOperation operation,
    ResultAnalysisState state,
    OperationBlockAnalysisContext context)
  {
    RegisterDiagnosticsForNonBranchingOperation(operation, state, context);
    ApplyStateChangesForNonBranchingOperation(operation, state);
    foreach (var childOperation in operation.ChildOperations)
      AnalyzeNonBranchingOperation(childOperation, state, context);
  }

  private static (ResultAnalysisState trueState, ResultAnalysisState falseState) AnalyzeBranchingOperation(
    IOperation operation,
    ResultAnalysisState state,
    OperationBlockAnalysisContext context)
  {
    // TODO: RegisterDiagnosticsForNonBranchingOperation(operation, state, context);
    if (IsChangingReferenceSymbolStateBranching(operation, state, out var symbol, out var trueState, out var falseState))
      return (state.With(symbol, trueState), state.With(symbol, falseState));
    // TODO: recursively go into the operations and register diagnostics

    return (state.Copy(), state.Copy());
  }

  private static void RegisterDiagnosticsForNonBranchingOperation(
    IOperation operation,
    ResultAnalysisState state,
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

  private static void ApplyStateChangesForNonBranchingOperation(IOperation operation, ResultAnalysisState state)
  {
    if (IsChangingReferenceSymbolState(operation, state, out var symbol, out var newState))
      state[symbol] = newState;
  }

  private static bool IsChangingReferenceSymbolState(IOperation operation,
    ResultAnalysisState state,
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

  private static readonly
    Dictionary<ResultTypeOperationIdentifier, (ResultTypeInstanceState trueState, ResultTypeInstanceState falseState)>
    BranchingOperationSymbolStates = new()
    {
      { ResultTypeOperationIdentifier.IsError, (ResultTypeInstanceState.Failure, ResultTypeInstanceState.Success) },
      { ResultTypeOperationIdentifier.IsSuccess, (ResultTypeInstanceState.Success, ResultTypeInstanceState.Failure) },
    };

  private static bool IsChangingReferenceSymbolStateBranching(IOperation operation,
    ResultAnalysisState state,
    [NotNullWhen(true)] out ISymbol? symbolOperation,
    out ResultTypeInstanceState trueState,
    out ResultTypeInstanceState falseState)
  {
    if (operation.IsOperationOnResultTypeInstance(out var identifier, out var instanceOperation) &&
        instanceOperation.IsSymbolReference(out symbolOperation) &&
        BranchingOperationSymbolStates.TryGetValue(identifier, out var states))
    {
      trueState = states.trueState;
      falseState = states.falseState;
      return true;
    }

    symbolOperation = null;
    trueState = default;
    falseState = default;
    return false;
  }

  private static ResultTypeInstanceState GetStateOfOperation(IOperation operation, ResultAnalysisState state)
  {
    if (operation.IsSymbolReference(out var symbol))
    {
      return state[symbol] switch
      {
        ResultTypeInstanceState.Success => ResultTypeInstanceState.Success,
        ResultTypeInstanceState.Failure => ResultTypeInstanceState.Failure,
        _ => ResultTypeInstanceState.Unknown,
      };
    }

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