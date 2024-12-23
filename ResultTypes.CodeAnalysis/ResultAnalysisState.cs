using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ResultTypes.CodeAnalysis;

internal class ResultAnalysisState
{
  private readonly Dictionary<ISymbol, ResultTypeInstanceState> stateDictionary = new(SymbolEqualityComparer.Default);

  private ResultAnalysisState() { }

  private ResultAnalysisState(IDictionary<ISymbol, ResultTypeInstanceState> data)
  {
    stateDictionary = new Dictionary<ISymbol, ResultTypeInstanceState>(data, SymbolEqualityComparer.Default);
  }

  internal ResultTypeInstanceState this[ISymbol symbol]
  {
    get =>
      stateDictionary.TryGetValue(symbol, out var value)
        ? value
        : ResultTypeInstanceState.Unchanged;
    set
    {
      if (value is ResultTypeInstanceState.Unchanged)
        stateDictionary.Remove(symbol);
      else
        stateDictionary[symbol] = value;
    }
  }

  internal bool IsEquivalentTo(ResultAnalysisState other) =>
    stateDictionary.OrderBy(entry => entry.Key).SequenceEqual(
      other.stateDictionary.OrderBy(entry => entry.Key));

  internal ResultAnalysisState Copy() => new(stateDictionary);

  internal ResultAnalysisState With(ISymbol symbol, ResultTypeInstanceState state)
  {
    var result = Copy();
    result[symbol] = state;
    return result;
  }

  internal ResultAnalysisState Combine(ResultAnalysisState other) => Combine(this, other);

  internal static ResultAnalysisState Combine(params ResultAnalysisState[] states) => Combine(states.AsEnumerable());
  
  internal static ResultAnalysisState Combine(IEnumerable<ResultAnalysisState> states) =>
    states.ToList() switch
    {
      [] => new ResultAnalysisState(),
      [var onlyState] => onlyState.Copy(),
      [var firstState, ..] => new ResultAnalysisState(states
        .Skip(1)
        .Aggregate(
          new HashSet<KeyValuePair<ISymbol, ResultTypeInstanceState>>(firstState.stateDictionary),
          (intersection, state) =>
          {
            intersection.IntersectWith(state.stateDictionary);
            return intersection;
          })
        .ToDictionary(pair => pair.Key, pair => pair.Value, SymbolEqualityComparer.Default)),
    };
}