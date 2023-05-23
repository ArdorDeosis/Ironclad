using Microsoft.CodeAnalysis;

namespace ResultTypes.CodeAnalysis;

/// <summary>
/// Representation of a Result type with success and failure value types. 
/// </summary>
internal readonly struct ResultTypeSymbol
{
	/// <summary>
	/// The success value type. Null, if the Result has no success value type.
	/// </summary>
	internal required INamedTypeSymbol? ValueType { get; init; }
	
	/// <summary>
	/// The failure value type.
	/// </summary>
	internal required INamedTypeSymbol ErrorType { get; init; }
	
	/// <summary>
	/// The original <see cref="INamedTypeSymbol"/> representing the result type.
	/// </summary>
	internal required INamedTypeSymbol Source { get; init; }
}