using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

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

	private void AnalyzeOperationBlocks(OperationBlockAnalysisContext context)
	{
		foreach (var operationBlock in context.OperationBlocks)
		{
			Console.WriteLine("### NEW OPERATION BLOCK ###");
			var graph = context.GetControlFlowGraph(operationBlock);
			var transitionedStates =
				new Dictionary<BasicBlock /* to */
					, Dictionary<BasicBlock /* from */, Dictionary<ISymbol, ResultTypeInstanceState>>>();
			var blocksToBeAnalyzed = new List<BasicBlock>(graph.Blocks);
			Console.WriteLine($"found {blocksToBeAnalyzed.Count} blocks to analyze");

			// iterate all blocks until you find one that has all predecessor block-state-transitions or the list is empty
			while (blocksToBeAnalyzed.Find(block => AllPredecessorStatesAreCalculated(block, transitionedStates)) is
				       { } block &&
			       blocksToBeAnalyzed.Remove(block))
			{
				Console.WriteLine($"analyzing block {block.Ordinal}");
				// get state from block-state-transition dictionary (combine multiple if necessary)
				IEnumerable<Dictionary<ISymbol, ResultTypeInstanceState>> incomingStates =
					transitionedStates.TryGetValue(block, out var stateDict)
						? stateDict.Values
						: Array.Empty<Dictionary<ISymbol, ResultTypeInstanceState>>();
				var initialState = CombineStates(incomingStates);
				PrintState(initialState);

				foreach (var operation in block.Operations)
					AnalyzeNonBranchingOperation(operation, ref initialState, context);

				if (block.BranchValue is not null) { }

				// saving state(s) (condition-separated) to block-state-transition dictionary

				if (block is { FallThroughSuccessor.Destination: { } fallthrough })
				{
					if (!transitionedStates.ContainsKey(fallthrough))
						transitionedStates.Add(fallthrough,
							new Dictionary<BasicBlock, Dictionary<ISymbol, ResultTypeInstanceState>>());
					transitionedStates[fallthrough].Add(block,
						new Dictionary<ISymbol, ResultTypeInstanceState>(initialState, SymbolEqualityComparer.Default));
				}

				if (block is { ConditionalSuccessor.Destination: { } conditional })
				{
					if (!transitionedStates.ContainsKey(conditional))
						transitionedStates.Add(conditional,
							new Dictionary<BasicBlock, Dictionary<ISymbol, ResultTypeInstanceState>>());
					if (!transitionedStates[conditional].ContainsKey(block)) // TODO: why is this necessary?
						transitionedStates[conditional].Add(block,
							new Dictionary<ISymbol, ResultTypeInstanceState>(initialState, SymbolEqualityComparer.Default));
				}
			}
		}

		// checks whether all predecessor states are 
		bool AllPredecessorStatesAreCalculated(BasicBlock block,
			Dictionary<BasicBlock, Dictionary<BasicBlock, Dictionary<ISymbol, ResultTypeInstanceState>>>
				transitionedStates) =>
			block.Predecessors.Length == 0
			|| transitionedStates.TryGetValue(block, out var incomingStates)
			&& block
				.Predecessors
				.Select(branch => branch.Source)
				.All(predecessorBlock => incomingStates.ContainsKey(predecessorBlock));

		Dictionary<ISymbol, ResultTypeInstanceState> CombineStates(
			IEnumerable<Dictionary<ISymbol, ResultTypeInstanceState>> states) =>
			!states.Any()
				? new Dictionary<ISymbol, ResultTypeInstanceState>(SymbolEqualityComparer.Default)
				: states
					.Skip(1)
					.Aggregate(
						new HashSet<KeyValuePair<ISymbol, ResultTypeInstanceState>>(states.First(),
							SymbolResultStatePairEqualityComparer.Default),
						(set, dict) =>
						{
							set.IntersectWith(dict);
							return set;
						}
					)
					.ToDictionary(pair => pair.Key, pair => pair.Value, SymbolEqualityComparer.Default);

		void PrintState(Dictionary<ISymbol, ResultTypeInstanceState> state)
		{
			Console.Write("state:");
			if (state.Count is 0)
				Console.WriteLine(" empty");
			foreach (var singleState in state)
				Console.WriteLine($"  {singleState.Key}: {singleState.Value}");
		}
	}

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

		if (operation.IsStaticOperationOnResultType(out var operationIdentifier))
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

	private class SymbolResultStatePairEqualityComparer : EqualityComparer<KeyValuePair<ISymbol, ResultTypeInstanceState>>
	{
		public new static SymbolResultStatePairEqualityComparer Default { get; } = new();

		public override bool Equals(KeyValuePair<ISymbol, ResultTypeInstanceState> x,
			KeyValuePair<ISymbol, ResultTypeInstanceState> y) =>
			x.Key.Equals(y.Key, SymbolEqualityComparer.Default) && x.Value.Equals(y.Value);

		public override int GetHashCode(KeyValuePair<ISymbol, ResultTypeInstanceState> obj) =>
			new { symbolHash = SymbolEqualityComparer.Default.GetHashCode(obj.Key), obj.Value }.GetHashCode();
	}
}