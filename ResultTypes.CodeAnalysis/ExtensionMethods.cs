using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace ResultTypes.CodeAnalysis;

internal static class ExtensionMethods
{
	internal static bool IsStaticOperationOnResultType(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier)
	{
		operationIdentifier = ResultTypeOperationIdentifier.None;
		return operation.IsMethodInvocationOnResultType(out operationIdentifier, out _) ||
		       operation.IsPropertyReferenceOnResultType(out operationIdentifier, out _);
	}

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

	internal static bool IsConversionOfResultType(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier,
		[NotNullWhen(true)] out IOperation? instanceOperation)
	{
		if (operation is IConversionOperation
		    {
			    Operand: { Type: { } typeConvertedFrom } operand,
			    Type: { } typeConvertedTo,
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

	internal static bool IsMethodInvocationOnResultTypeInstance(this IOperation operation,
		out ResultTypeOperationIdentifier operationIdentifier,
		[NotNullWhen(true)] out IOperation? instanceOperation)
	{
		operationIdentifier = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return operation.IsMethodInvocationOnResultType(out operationIdentifier, out instanceOperation)
		       && instanceOperation is not null;
	}

	internal static bool IsPropertyReferenceOnResultType(this IOperation operation,
		out ResultTypeOperationIdentifier operationDescriptor,
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
			operationDescriptor = propertyName.ToResultTypeOperationIdentifier();
			instanceOperation = instance;
			return true;
		}

		operationDescriptor = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return false;
	}

	internal static bool IsPropertyReferenceOnResultTypeInstance(this IOperation operation,
		out ResultTypeOperationIdentifier operationDescriptor,
		[NotNullWhen(true)] out IOperation? instanceOperation)
	{
		operationDescriptor = ResultTypeOperationIdentifier.None;
		instanceOperation = null;
		return operation.IsPropertyReferenceOnResultType(out operationDescriptor, out instanceOperation) &&
		       instanceOperation is not null;
	}


	internal static bool IsAssignmentToResultTypeReferenceSymbol(this IOperation operation,
		[NotNullWhen(true)] out ISymbol? symbol,
		[NotNullWhen(true)] out IOperation? valueOperation)
	{
		if (operation is IAssignmentOperation { Target: { Type: { } targetType } target, Value: { } value }
		    && targetType.IsResultType(out _)
		    && target.IsSymbolReference(out symbol))
		{
			valueOperation = value;
			return true;
		}

		valueOperation = default;
		symbol = default;
		return false;
	}

	internal static bool IsConversionOfResultTypeReferenceSymbolToValueType(this IOperation operation,
		[NotNullWhen(true)] out ISymbol? symbol)
	{
		symbol = null;
		return operation is IConversionOperation
		       {
			       Operand: { Type: { } typeConvertedFrom } operand,
			       Type: { } typeConvertedTo,
		       }
		       && typeConvertedFrom.IsResultType(out var resultTypeSymbol)
		       && operand.IsSymbolReference(out symbol)
		       && typeConvertedTo.Equals(resultTypeSymbol.ValueType, SymbolEqualityComparer.Default);
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

	internal static bool IsResultTypeInvocation(this IOperation? operation,
		[NotNullWhen(true)] out IInvocationOperation? invocationOperation)
	{
		invocationOperation = operation as IInvocationOperation;
		return invocationOperation is not null && invocationOperation.TargetMethod.ContainingType.IsResultType(out _);
	}

	internal static bool IsResultTypePropertyReference(this IOperation? operation,
		[NotNullWhen(true)] out IPropertyReferenceOperation? propertyReferenceOperation)
	{
		propertyReferenceOperation = operation as IPropertyReferenceOperation;
		return propertyReferenceOperation is not null &&
		       propertyReferenceOperation.Property.ContainingType.IsResultType(out _);
	}

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
}