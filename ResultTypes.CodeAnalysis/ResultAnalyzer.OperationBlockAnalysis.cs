using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace ResultTypes.CodeAnalysis;

internal sealed partial class ResultAnalyzer
{
	/// <summary>
	/// Analyze all operation blocks in the given context.
	/// </summary>
	/// <param name="context">The context whose operation blocks should be analyzed.</param>
	private void AnalyzeOperationBlocks(OperationBlockAnalysisContext context)
	{
		foreach (var operationBlock in context.OperationBlocks)
			AnalyzeOperationBlock(operationBlock, context);
	}

	private void AnalyzeOperationBlock(IOperation operationBlock, OperationBlockAnalysisContext context)
	{
		// This is what needs to happen here:
		// 
		// let queue be a queue of basic blocks
		// let analyzed be a map mapping from basic blocks to states
		//
		// enqueue start block
		//
		// while (queue is not empty)
		// 	 let block = queue.pop()
		//   let inputState = getInputState // (as much as is available)
		//   if (already analyzed with inputState)
		//   continue;
		//   analyze(block)
		//   add to analyzed with inputState
		// 	 enqueue childern
		
		var graph = context.GetControlFlowGraph(operationBlock);
		
		var blocksToBeAnalyzed = new List<BasicBlock>(graph.Blocks);

		var stateTransitions =
			new Dictionary<(BasicBlock from, BasicBlock to), Dictionary<ISymbol, ResultTypeInstanceState>>();

		while (TryGetNextBlockToAnalyze(out var block, out var inputState))
		{
			blocksToBeAnalyzed.Remove(block);
			
			foreach (var operation in block.Operations)
				AnalyzeNonBranchingOperation(operation, ref inputState, context);

			// block is not branching
			if (block.BranchValue is not null) { }

			// saving state(s) (condition-separated) to block-state-transition dictionary

			// if (block is { FallThroughSuccessor.Destination: { } fallthrough })
			// {
			// 	if (!transitionedStates.ContainsKey(fallthrough))
			// 		transitionedStates.Add(fallthrough,
			// 			new Dictionary<BasicBlock, Dictionary<ISymbol, ResultTypeInstanceState>>());
			// 	transitionedStates[fallthrough].Add(block,
			// 		new Dictionary<ISymbol, ResultTypeInstanceState>(initialState, SymbolEqualityComparer.Default));
			// }
			//
			// if (block is { ConditionalSuccessor.Destination: { } conditional })
			// {
			// 	if (!transitionedStates.ContainsKey(conditional))
			// 		transitionedStates.Add(conditional,
			// 			new Dictionary<BasicBlock, Dictionary<ISymbol, ResultTypeInstanceState>>());
			// 	if (!transitionedStates[conditional].ContainsKey(block)) // TODO: why is this necessary?
			// 		transitionedStates[conditional].Add(block,
			// 			new Dictionary<ISymbol, ResultTypeInstanceState>(initialState, SymbolEqualityComparer.Default));
			// }
		}
		
		bool TryGetNextBlockToAnalyze(
			[NotNullWhen(true)] out BasicBlock? block,
			[NotNullWhen(true)] out Dictionary<ISymbol, ResultTypeInstanceState>? inputState)
		{
			foreach (var blockToCheck in blocksToBeAnalyzed)
			{
				block = blockToCheck;
				if (TryGetInputStateForBlock(blockToCheck, out inputState))
					return true;
			}

			block = default;
			inputState = default;
			return false;
		}

		bool TryGetInputStateForBlock(BasicBlock block,
			[NotNullWhen(true)] out Dictionary<ISymbol, ResultTypeInstanceState>? inputState)
		{
			inputState = new Dictionary<ISymbol, ResultTypeInstanceState>(SymbolEqualityComparer.Default);
			if (block.Predecessors.Length is 0) 
				return true;

			var incomingStates = block.Predecessors
				.Select(branch => stateTransitions.TryGetValue((branch.Source, block), out var incomingState)
					? incomingState
					: null).ToArray();
			return TryCombineStates(incomingStates, out inputState);
		}

		static bool TryCombineStates(
			IList<Dictionary<ISymbol, ResultTypeInstanceState>?> states,
			[NotNullWhen(true)] out Dictionary<ISymbol, ResultTypeInstanceState>? combinedState)
		{
			combinedState = states.Any(state => state is null)
				? null
				: states switch
				{
					[] => new Dictionary<ISymbol, ResultTypeInstanceState>(SymbolEqualityComparer.Default),
					[{ } onlyState] => new Dictionary<ISymbol, ResultTypeInstanceState>(
						onlyState, SymbolEqualityComparer.Default),
					[{ } firstState, ..] => states
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
			return combinedState is not null;
		}
	}
}