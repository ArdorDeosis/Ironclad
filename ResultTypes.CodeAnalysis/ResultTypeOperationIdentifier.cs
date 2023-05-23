namespace ResultTypes.CodeAnalysis;

internal enum ResultTypeOperationIdentifier
{
	None,
	SuccessConstructor,
	ErrorConstructor,
	ConversionToSuccessValue,
	OrFallback,
	OrThrow,
	IsError,
	IsSuccess,
}

internal static class ResultTypeOperationExtensions
{
	
	internal static ResultTypeOperationIdentifier ToResultTypeOperationIdentifier(this string memberName) => memberName switch
	{
		"Success" => ResultTypeOperationIdentifier.SuccessConstructor,
		"Error" => ResultTypeOperationIdentifier.ErrorConstructor,
		"Or" or "OrDefault" => ResultTypeOperationIdentifier.OrFallback,
		"OrThrow" => ResultTypeOperationIdentifier.OrThrow,
		"IsError" => ResultTypeOperationIdentifier.IsError,
		"IsSuccess" => ResultTypeOperationIdentifier.IsSuccess,
		_ => ResultTypeOperationIdentifier.None,
	};
}