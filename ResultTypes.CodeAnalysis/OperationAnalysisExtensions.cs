using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace ResultTypes.CodeAnalysis;

/// <summary>
/// Operation analysis extension methods.
/// </summary>
internal static class OperationAnalysisExtensions
{
	/// <summary>
	/// Whether the operation is a reference to a named symbol (field, local, parameter).
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="symbol">The symbol referenced by the operation.</param>
	internal static bool IsSymbolReference(this IOperation operation, [NotNullWhen(true)] out ISymbol? symbol)
	{
		symbol = operation switch
		{
			IFieldReferenceOperation { Field: { } field } => field,
			ILocalReferenceOperation { Local: { } local } => local,
			IParameterReferenceOperation { Parameter: { } parameter } => parameter,
			_ => null,
		};
		return symbol is not null;
	}

	/// <summary>
	/// Whether the given type is an Ironclad.ResultTypes.Result.
	/// </summary>
	/// <param name="symbol">The type symbol to check.</param>
	/// <param name="resultTypeSymbol">A struct representing relevant type information about the result type.</param>
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
			if (resultType is
			    {
				    TypeParameters: [{ Name: "TValue" }, { Name: "TError" }],
				    TypeArguments: [INamedTypeSymbol value, INamedTypeSymbol error],
			    })
			{
				resultTypeSymbol = new ResultTypeSymbol
				{
					ValueType = value,
					ErrorType = error,
					Source = resultType,
				};
				return true;
			}

			if (resultType is
			    {
				    TypeParameters: [{ Name: "TError" }],
				    TypeArguments: [INamedTypeSymbol errorType],
			    })
			{
				resultTypeSymbol = new ResultTypeSymbol
				{
					ValueType = null,
					ErrorType = errorType,
					Source = resultType,
				};
				return true;
			}
		}

		resultTypeSymbol = default!;
		return false;
	}

	/// <summary>
	/// Whether the given operation is an operation on a result type.
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="operationIdentifier">The type of the given operation on a result type.</param>
	internal static bool IsOperationOnResultType(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier)
	{
		operationIdentifier = ResultTypeOperationIdentifier.None;
		return operation.IsMethodInvocationOnResultType(out operationIdentifier, out _) ||
		       operation.IsPropertyReferenceOnResultType(out operationIdentifier, out _);
	}

	/// <summary>
	/// Whether the given operation is an operation on a named symbol of a result type (field, local parameter).
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="operationIdentifier">The type of the given operation on a result type.</param>
	/// <param name="instanceOperation">The symbol referenced by the operation.</param>
	internal static bool IsOperationOnResultTypeInstance(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier,
		[NotNullWhen(true)] out IOperation? instanceOperation)
	{
		operationIdentifier = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return operation.IsConversionOfResultType(out operationIdentifier, out instanceOperation) ||
		       operation.IsMethodInvocationOnResultTypeInstance(out operationIdentifier, out instanceOperation) ||
		       operation.IsPropertyReferenceOnResultTypeInstance(out operationIdentifier, out instanceOperation);
	}

	/// <summary>
	/// Whether the given operation is a conversion operation of a result type to another type.
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="operationIdentifier">The type of the given operation on a result type.</param>
	/// <param name="instanceOperation">The symbol referenced by the operation.</param>
	internal static bool IsConversionOfResultType(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier,
		[NotNullWhen(true)] out IOperation? instanceOperation)
	{
		if (operation is IConversionOperation
		    {
			    Operand: { Type: { } typeConvertedFrom } operand,
			    Type: { Name: {} typeName } typeConvertedTo,
		    }
		    && typeConvertedFrom.IsResultType(out var resultTypeSymbol)
		    && typeConvertedTo.Equals(resultTypeSymbol.ValueType, SymbolEqualityComparer.Default))
		{
			operationIdentifier = ResultTypeOperationIdentifier.ConversionToSuccessValue;
			instanceOperation = operand;
			return true;
		}

		operationIdentifier = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return false;
	}

	/// <summary>
	/// Whether the given operation is a conversion of a named symbol of a result type  (field, local parameter) to the
	/// result types success value type.
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="symbol">The symbol referenced by the operation.</param>
	/// <returns></returns>
	internal static bool IsConversionOfResultTypeReferenceSymbolToValueType(this IOperation operation,
		[NotNullWhen(true)] out ISymbol? symbol)
	{
		symbol = null;
		return operation.IsConversionOfResultType(out var operationIdentifier, out var operand)
		       && operationIdentifier is ResultTypeOperationIdentifier.ConversionToSuccessValue
		       && operand.IsSymbolReference(out symbol);
	}

	/// <summary>
	/// Whether the given operation is a method invocation on a result type.
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="operationIdentifier">The type of the given operation on a result type.</param>
	/// <param name="instanceOperation">The symbol referenced by the operation or null if the method is static.</param>
	internal static bool IsMethodInvocationOnResultType(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier,
		out IOperation? instanceOperation)
	{
		if (operation is IInvocationOperation
		    {
			    Instance: var instance,
			    TargetMethod:
			    {
				    Name: { } methodName,
				    ContainingType: { } type,
			    },
		    } && type.IsResultType(out _))
		{
			operationIdentifier = methodName.ToResultTypeOperationIdentifier();
			instanceOperation = instance;
			return true;
		}

		operationIdentifier = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return false;
	}

	/// <summary>
	/// Whether the given operation is a method invocation on a result type.
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="operationIdentifier">The type of the given operation on a result type.</param>
	/// <param name="instanceOperation">The symbol referenced by the operation.</param>
	internal static bool IsMethodInvocationOnResultTypeInstance(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier,
		[NotNullWhen(true)] out IOperation? instanceOperation)
	{
		operationIdentifier = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return operation.IsMethodInvocationOnResultType(out operationIdentifier, out instanceOperation)
		       && instanceOperation is not null;
	}

	/// <summary>
	/// Whether the given operation is a property reference on a result type.
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="operationIdentifier">The type of the given operation on a result type.</param>
	/// <param name="instanceOperation">The symbol referenced by the operation or null if the property is static.</param>
	internal static bool IsPropertyReferenceOnResultType(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier,
		out IOperation? instanceOperation)
	{
		if (operation is IPropertyReferenceOperation
		    {
			    Instance: var instance,
			    Property:
			    {
				    Name: { } propertyName,
				    ContainingType: { } type,
			    },
		    } && type.IsResultType(out _))
		{
			operationIdentifier = propertyName.ToResultTypeOperationIdentifier();
			instanceOperation = instance;
			return true;
		}

		operationIdentifier = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return false;
	}

	
	/// <summary>
	/// Whether the given operation is a property reference on a result type.
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="operationIdentifier">The type of the given operation on a result type.</param>
	/// <param name="instanceOperation">The symbol referenced by the operation.</param>
	internal static bool IsPropertyReferenceOnResultTypeInstance(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier,
		[NotNullWhen(true)] out IOperation? instanceOperation)
	{
		operationIdentifier = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return operation.IsPropertyReferenceOnResultType(out operationIdentifier, out instanceOperation) &&
		       instanceOperation is not null;
	}

	/// <summary>
	/// Whether the given operation is an assignment to a named symbol of a result type (field, local, parameter).
	/// </summary>
	/// <param name="operation">The operation to check.</param>
	/// <param name="instanceSymbol">The symbol referenced by the operation.</param>
	/// <param name="valueOperation">The value operation assigned to the symbol.</param>
	internal static bool IsAssignmentToResultTypeReferenceSymbol(this IOperation operation,
		[NotNullWhen(true)] out ISymbol? instanceSymbol,
		[NotNullWhen(true)] out IOperation? valueOperation)
	{
		if (operation is IAssignmentOperation { Target: { Type: { } targetType } target, Value: { } value }
		    && targetType.IsResultType(out _)
		    && target.IsSymbolReference(out instanceSymbol))
		{
			valueOperation = value;
			return true;
		}

		valueOperation = default;
		instanceSymbol = default;
		return false;
	}
}