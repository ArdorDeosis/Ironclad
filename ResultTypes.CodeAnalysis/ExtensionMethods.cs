﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ResultTypes.CodeAnalysis;

internal readonly struct ResultTypeSymbol
{
  internal required INamedTypeSymbol? ValueType { get; init; }
  internal required INamedTypeSymbol ErrorType { get; init; }
  internal required INamedTypeSymbol Source { get; init; }
}

internal static class ExtensionMethods
{
  // TODO:
  internal static bool HasFixedResultState(this IOperation operation, out ResultState resultState)
  {
    resultState = default;
    return false;
  }

  internal static bool IsTrackableSymbol(this IOperation operation, [NotNullWhen(true)] out ISymbol? symbol)
  {
    symbol = operation switch
    {
      ILocalReferenceOperation { Local: { } local } => local,
      IParameterReferenceOperation { Parameter: { } parameter } => parameter,
      IFieldReferenceOperation { Field: { } field } => field,
      _ => null,
    };
    return symbol is not null;
  }

  internal static ResultState GetResultState(this IOperation operation, IDictionary<ISymbol, ResultState>? state = null)
  {
    // TODO: add 'fixed' states, e.g. Result.Success() or implicit conversion
    
    ISymbol? lookupSymbol = operation switch
    {
      ILocalReferenceOperation { Local: { } local } => local,
      IParameterReferenceOperation { Parameter: { } parameter } => parameter,
      IFieldReferenceOperation { Field: { } field } => field,
      _ => null,
    };

    return lookupSymbol is not null && state is not null && state.TryGetValue(lookupSymbol, out var resultState)
      ? resultState
      : ResultState.Unknown;
  }
  
  internal static bool IsResultType([NotNullWhen(true)] this ITypeSymbol? symbol, out ResultTypeSymbol resultTypeSymbol)
  {

    if (symbol is INamedTypeSymbol
        {
          Kind: SymbolKind.NamedType,
          Name: "Result",
          ContainingNamespace:
          {
            Name: "ResultTypes",
            ContainingNamespace.Name: "Ironclad",
          },
        } resultType)
    {
      switch (resultType)
      {
        case 
        {
          TypeParameters: [{ Name: "TValue" }, { Name: "TError" }],
          TypeArguments: [INamedTypeSymbol value, INamedTypeSymbol error],
        }:
          resultTypeSymbol = new ResultTypeSymbol
          {
            ValueType = value,
            ErrorType = error,
            Source = resultType,
          };
          return true;
        case
        {
          TypeParameters: [{ Name: "TError" }],
          TypeArguments: [INamedTypeSymbol othererror],
        }:
          resultTypeSymbol = new ResultTypeSymbol
          {
            ValueType = null,
            ErrorType = othererror,
            Source = resultType,
          };
          return true;
      }
    }

    resultTypeSymbol = default!;
    return false;
  }

  internal static bool IsResultTypeWithValue([NotNullWhen(true)] this ITypeSymbol? symbol) =>
    symbol.IsResultType(out var resultType) &&
    resultType.ValueType is not null;

  internal static bool IsResultTypeWithoutValue([NotNullWhen(true)] this ITypeSymbol? symbol) =>
    symbol.IsResultType(out var resultType) &&
    resultType.ValueType is null;

  internal static bool IsResultTypeInvocation(this OperationAnalysisContext context,
    [NotNullWhen(true)] out IInvocationOperation? invocation) =>
    context.Operation.IsResultTypeInvocation(out invocation);

  internal static bool IsResultTypeInvocation(this IOperation? operation,
    [NotNullWhen(true)] out IInvocationOperation? invocation)
  {
    invocation = operation as IInvocationOperation;
    return invocation is not null && invocation.TargetMethod.ContainingType.IsResultType(out _);
  }

  internal static bool IsResultTypePropertyReference(this OperationAnalysisContext context,
    [NotNullWhen(true)] out IPropertyReferenceOperation? invocation) =>
    IsResultTypePropertyReference(context.Operation, out invocation);

  internal static bool IsResultTypePropertyReference(this IOperation? operation,
    [NotNullWhen(true)] out IPropertyReferenceOperation? invocation)
  {
    invocation = operation as IPropertyReferenceOperation;
    return invocation is not null && invocation.Property.ContainingType.IsResultType(out _);
  }

  internal static bool IsDefiningResultState(this IOperation operation,
    [NotNullWhen(true)] out ISymbol? symbol,
    out ResultState state)
  {
    // implicit conversion to value
    if (operation.IsResultToValueConversionFromLocalOrFieldOrParameter(out _) &&
        operation.IsConversionFromLocalOrFieldOrParameter(out symbol))
    {
      state = ResultState.Success;
      return true;
    }

    // OrThrow()
    // IsError(), IsError(out TError)
    if (operation.IsResultTypeInvocation(out var invocation) &&
        invocation.Instance is { } instance &&
        instance.IsLocalOrField(out symbol))
    {
      switch (invocation.TargetMethod.Name)
      {
        case "OrThrow":
          state = ResultState.Success;
          return true;
        case "IsError":
          state = ResultState.Error;
          return true;
      }
    }


    // assignments
    if (operation is IAssignmentOperation assignment &&
        assignment.Target.Type.IsResultType(out _) &&
        assignment.Target.IsLocalOrField(out symbol) &&
        assignment.Value.Type.IsResultType(out var resultType))
    {
      // assigning .Success() or .Error()
      if (assignment.Value.IsResultTypeInvocation(out var valueInvocation))
      {
        switch (valueInvocation.TargetMethod.Name)
        {
          case "Success":
            state = ResultState.Success;
            return true;
          case "Error":
            state = ResultState.Error;
            return true;
        }
      }

      // assigning .Success property
      if (assignment.Value.IsResultTypePropertyReference(out var propertyReference) &&
          propertyReference.Property.Name == "Success")
      {
        state = ResultState.Success;
        return true;
      }

      // assigning implicit conversion
      if (assignment.Value is IConversionOperation
          {
            Operand.Type: {} fromType,
          })
      {
        // implicitly convert success
        if (fromType.Equals(resultType.ValueType, SymbolEqualityComparer.Default))
        {
          state = ResultState.Success;
          return true;
        }
        // implicitly convert error
        if (fromType.Equals(resultType.ErrorType, SymbolEqualityComparer.Default))
        {
          state = ResultState.Error;
          return true;
        }
      }
    }


    state = ResultState.Unknown;
    symbol = null;
    return false;
  }
  
  

  internal static bool IsSymbolReference(this IOperation operation, [NotNullWhen(true)] out ISymbol? symbol)
  {
    symbol = operation switch
    {
      IFieldReferenceOperation { Field: { } field } => field,
      ILocalReferenceOperation { Local: { } local } => local,
      IParameterReferenceOperation { Parameter: { } parameter } => parameter,
      _=> null,
    };
    return symbol is not null;
  }

  internal static bool IsResultToValueConversion(this IOperation operation) =>
    operation is IConversionOperation { Operand.Type: INamedTypeSymbol operand } conversion &&
    operand.IsResultType(out var resultType) && resultType is { ValueType: { } valueType } &&
    valueType.Equals(conversion.Type, SymbolEqualityComparer.Default);

  internal static bool IsResultToValueConversionFromLocalOrFieldOrParameter(this IOperation operation, [NotNullWhen(true)] out ISymbol? symbol)
  {
    if (operation is IConversionOperation { Operand.Type: INamedTypeSymbol operand } conversion &&
        operand.IsResultType(out var resultType) && resultType is { ValueType: { } valueType } &&
        valueType.Equals(conversion.Type, SymbolEqualityComparer.Default) && 
        operation.IsConversionFromLocalOrFieldOrParameter(out var variable))
    {
      symbol = variable;
      return true;
    }
    symbol = default;
    return false;
  }

  internal static bool IsImplicitValueToSuccessConversion(this IOperation operation) =>
    operation is IConversionOperation
    {
      Type: INamedTypeSymbol
      {
        Name: "Result",
        ContainingNamespace:
        {
          Name: "ResultTypes",
          ContainingNamespace.Name: "Ironclad",
        },
      } resultType,
      Operand.Type: INamedTypeSymbol operandType,
    } &&
    resultType.TypeArguments[0].Equals(operandType, SymbolEqualityComparer.Default);

  internal static bool IsImplicitResultToBoolConversion(this IOperation operation) =>
    operation is IConversionOperation
    {
      Operand.Type: INamedTypeSymbol operand,
      Type.SpecialType: SpecialType.System_Boolean,
    } && operand.IsResultTypeWithoutValue();

  internal static bool IsConversionFromLocalOrFieldOrParameter(this IOperation operation, [NotNullWhen(true)] out ISymbol? symbol)
  {
    symbol = (operation as IConversionOperation)?.Operand switch
    {
      ILocalReferenceOperation { Local: { } local } => local,
      IFieldReferenceOperation { Field: { } field } => field,
      IParameterReferenceOperation { Parameter: { } parameter } => parameter,
      _ => null,
    };
    return symbol is not null;
  }

  internal static bool IsLocalOrField(this IOperation operation, [NotNullWhen(true)] out ISymbol? symbol)
  {
    symbol = operation switch
    {
      ILocalReferenceOperation { Local: { } local } => local,
      IFieldReferenceOperation { Field: { } field } => field,
      _ => null,
    };
    return symbol is not null;
  }
}