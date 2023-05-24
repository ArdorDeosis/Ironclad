using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace ResultTypes.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed partial class ResultAnalyzer : DiagnosticAnalyzer
{
	public override void Initialize(AnalysisContext context)
	{
		context.EnableConcurrentExecution();
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
		                                       GeneratedCodeAnalysisFlags.ReportDiagnostics);

		context.RegisterOperationBlockAction(AnalyzeOperationBlocks);
	}

	// checks whether all predecessor states are 

	static Dictionary<ISymbol, ResultTypeInstanceState> CombineStates(
		ICollection<Dictionary<ISymbol, ResultTypeInstanceState>> states) =>
		states.ToList() switch
		{
			[] => new Dictionary<ISymbol, ResultTypeInstanceState>(SymbolEqualityComparer.Default),
			[var onlyState] => new Dictionary<ISymbol, ResultTypeInstanceState>(
				onlyState, SymbolEqualityComparer.Default),
			[var firstState, ..] => states
				.Skip(1)
				.Aggregate(
					new HashSet<KeyValuePair<ISymbol, ResultTypeInstanceState>>(firstState),
					(intersection, state) =>
					{
						intersection.IntersectWith(state);
						return intersection;
					})
				.ToDictionary(pair => pair.Key, pair => pair.Value, SymbolEqualityComparer.Default),
		};

	void PrintState(Dictionary<ISymbol, ResultTypeInstanceState> state)
	{
		Console.Write("state:");
		if (state.Count is 0)
			Console.WriteLine(" empty");
		foreach (var singleState in state)
			Console.WriteLine($"  {singleState.Key}: {singleState.Value}");
	}

	internal class
		SymbolResultStatePairEqualityComparer : EqualityComparer<KeyValuePair<ISymbol, ResultTypeInstanceState>>
	{
		public new static SymbolResultStatePairEqualityComparer Default { get; } = new();

		public override bool Equals(KeyValuePair<ISymbol, ResultTypeInstanceState> x,
			KeyValuePair<ISymbol, ResultTypeInstanceState> y) =>
			x.Key.Equals(y.Key, SymbolEqualityComparer.Default) && x.Value.Equals(y.Value);

		public override int GetHashCode(KeyValuePair<ISymbol, ResultTypeInstanceState> obj) =>
			new { symbolHash = SymbolEqualityComparer.Default.GetHashCode(obj.Key), obj.Value }.GetHashCode();
	}
}