using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace ResultTypes.CodeAnalysis;

public enum ResultState
{
  Unknown,
  Error,
  Success,
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class ResultAnalyzer : DiagnosticAnalyzer
{
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    ImmutableArray.Create(Rule, ConversionOfError, ConversionOfUnknown);

  public static readonly DiagnosticDescriptor Rule = new("IC", "TITLE---", "format", "Naming",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description.");

  public static readonly DiagnosticDescriptor ConversionOfError = new("IC001",
    "conversion of failed Result",
    "conversion will throw exception, since {0} is a Result.Error", "Ironclad.ResultTypes",
    DiagnosticSeverity.Error, isEnabledByDefault: true, description: "description");

  public static readonly DiagnosticDescriptor ConversionOfUnknown = new("IC002",
    "conversion of unknown Result",
    "conversion of {0} might throw an exception if {0} is an Error", "Ironclad.ResultTypes",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description");

  // for test purposes
  private static readonly DiagnosticDescriptor UnreachableCode = new DiagnosticDescriptor(
    "UnreachableCode",
    "Unreachable code",
    "Unreachable code detected",
    "CodeQuality",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);

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
        new Dictionary<BasicBlock /* to */, Dictionary<BasicBlock /* from */, Dictionary<ISymbol, ResultState>>>();
      var blocksToBeAnalyzed = new List<BasicBlock>(graph.Blocks);
      Console.WriteLine($"found {blocksToBeAnalyzed.Count} blocks to analyze");

      // iterate all blocks until you find one that has all predecessor block-state-transitions or the list is empty
      while (blocksToBeAnalyzed.Find(block => AllPredecessorStatesAreCalculated(block, transitionedStates)) is
               { } block &&
             blocksToBeAnalyzed.Remove(block))
      {
        Console.WriteLine($"analyzing block {block.Ordinal}");
        // get state from block-state-transition dictionary (combine multiple if necessary)
        IEnumerable<Dictionary<ISymbol, ResultState>> incomingStates =
          transitionedStates.TryGetValue(block, out var stateDict)
            ? stateDict.Values
            : Array.Empty<Dictionary<ISymbol, ResultState>>();
        var initialState = CombineStates(incomingStates);
        PrintState(initialState);

        foreach (var operation in block.Operations)
          AnalyzeNonBranchingOperation(operation, initialState, context);

        if (block.BranchValue is not null) { }

        // saving state(s) (condition-separated) to block-state-transition dictionary

        if (block is { FallThroughSuccessor.Destination: { } fallthrough })
        {
          if (!transitionedStates.ContainsKey(fallthrough))
            transitionedStates.Add(fallthrough, new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
          transitionedStates[fallthrough].Add(block,
            new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
        }

        if (block is { ConditionalSuccessor.Destination: { } conditional })
        {
          if (!transitionedStates.ContainsKey(conditional))
            transitionedStates.Add(conditional, new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
          if (!transitionedStates[conditional].ContainsKey(block)) // TODO: why is this necessary?
            transitionedStates[conditional].Add(block,
              new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
        }
      }
    }

    // checks whether all predecessor states are 
    bool AllPredecessorStatesAreCalculated(BasicBlock block,
      Dictionary<BasicBlock, Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>> transitionedStates) =>
      block.Predecessors.Length == 0
      || transitionedStates.TryGetValue(block, out var incomingStates)
      && block
        .Predecessors
        .Select(branch => branch.Source)
        .All(predecessorBlock => incomingStates.ContainsKey(predecessorBlock));

    Dictionary<ISymbol, ResultState> CombineStates(IEnumerable<Dictionary<ISymbol, ResultState>> states) =>
      !states.Any()
        ? new Dictionary<ISymbol, ResultState>(SymbolEqualityComparer.Default)
        : states
          .Skip(1)
          .Aggregate(
            new HashSet<KeyValuePair<ISymbol, ResultState>>(states.First(),
              SymbolResultStatePairEqualityComparer.Default),
            (set, dict) =>
            {
              set.IntersectWith(dict);
              return set;
            }
          )
          .ToDictionary(pair => pair.Key, pair => pair.Value, SymbolEqualityComparer.Default);

    void PrintState(Dictionary<ISymbol, ResultState> state)
    {
      Console.Write("state:");
      if (state.Count is 0)
        Console.WriteLine(" empty");
      foreach (var singleState in state)
        Console.WriteLine($"  {singleState.Key}: {singleState.Value}");
    }
  }

  private delegate IEnumerable<IOperation> AnalyzeOperationDelegate(
    IOperation operation,
    IDictionary<ISymbol, ResultState> state,
    OperationBlockAnalysisContext context);

  private static AnalyzeOperationDelegate[] OperationAnalyzers { get; } =
  {
    // AnalyzeResultToValueConversion,
  };

  private static void AnalyzeOperation(IOperation operation, IDictionary<ISymbol, ResultState> state,
    OperationBlockAnalysisContext context)
  {
    Console.WriteLine($"analyzing {operation.Kind}: {operation.Syntax}");
    foreach (var operationAnalyzer in OperationAnalyzers)
    {
      var childOperations = operationAnalyzer(operation, state, context);
      foreach (var childOperation in childOperations)
        AnalyzeOperation(childOperation, state, context);
    }
  }

  private static void AnalyzeNonBranchingOperation(IOperation operation, IDictionary<ISymbol, ResultState> state,
    OperationBlockAnalysisContext context)
  {
    // is this requiring any state?
    RegisterDiagnosticsForNonBranchingOperation(operation, state, context);
    foreach (var childOperation in operation.ChildOperations)
      AnalyzeNonBranchingOperation(childOperation, state, context);

    // is this changing the state?
    // check for 
    //    assignments => GetStateOfOperation()
    //    conversion to value (if state converted from named symbol, state is true)
    //    OrThrow (if on named symbol, state is true)
  }

  private static void RegisterDiagnosticsForNonBranchingOperation(IOperation operation,
    IDictionary<ISymbol, ResultState> state,
    OperationBlockAnalysisContext context)
  {
    //  check for:
    //    conversion to value (GetStateOfOperation())
    //    Or(Default/Throw) (GetStateOfOperation())
    //    IsError/IsSuccess (GetStateOfOperation())


    if (operation is IConversionOperation
        {
          Operand: { Type: { } typeConvertedFrom } operand,
          Type: { } typeConvertedTo,
        }
        && typeConvertedFrom.IsResultType(out var resultTypeSymbol)
        && typeConvertedTo.Equals(resultTypeSymbol.ValueType, SymbolEqualityComparer.Default))
    {
      switch (GetStateOfOperation(operand, state))
      {
        case ResultState.Unknown:
          RegisterDiagnostic();
          context.ReportDiagnostic(Diagnostic.Create(ConversionOfUnknown, operation.Syntax.GetLocation(), operand.Syntax));
          break;
        case ResultState.Error:
          context.ReportDiagnostic(Diagnostic.Create(ConversionOfError, operation.Syntax.GetLocation(), operand.Syntax));
          break;
      }
    }

    if (operation is IInvocationOperation { Instance: { Type: { } type } instance } invocation
        && type.IsResultType(out _))
    {
      switch ((invocation.TargetMethod.Name, GetStateOfOperation(instance, state)))
      {
        case ("Or", ResultState.Error):
          RegisterDiagnostic();
          break;
        case ("Or", ResultState.Success):
          RegisterDiagnostic();
          break;
        case ("OrDefault", ResultState.Error):
          RegisterDiagnostic();
          break;
        case ("OrDefault", ResultState.Success):
          RegisterDiagnostic();
          break;
        case ("OrThrow", ResultState.Error):
          RegisterDiagnostic();
          break;
        case ("OrThrow", ResultState.Success):
          RegisterDiagnostic();
          break;
        case ("IsError", ResultState.Error):
          RegisterDiagnostic();
          break;
        case ("IsError", ResultState.Success):
          RegisterDiagnostic();
          break;
        case ("IsSuccess", ResultState.Error):
          RegisterDiagnostic();
          break;
        case ("IsSuccess", ResultState.Success):
          RegisterDiagnostic();
          break;
      }
    }

    // TODO: add more pattern matching
    if (operation is IPropertyReferenceOperation { } propertyReference
        && propertyReference.Instance.Type.IsResultType(out _))
    {
      // TODO: this reeks of a table
      switch (propertyReference.Property.Name, GetStateOfOperation(propertyReference.Instance, state))
      {
        case ("IsError", ResultState.Error):
          RegisterDiagnostic();
          break;
        case ("IsError", ResultState.Success):
          RegisterDiagnostic();
          break;
        case ("IsSuccess", ResultState.Error):
          RegisterDiagnostic();
          break;
        case ("IsSuccess", ResultState.Success):
          RegisterDiagnostic();
          break;
      }
    }
  }

  private static void RegisterDiagnostic()
  {
    // TODO: replace with actual diagnostics
  }

  private static ResultState GetStateOfOperation(IOperation operation, IDictionary<ISymbol, ResultState> state)
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
        return ResultState.Error;
      if (operandType.Equals(resultTypeSymbol.ValueType, SymbolEqualityComparer.Default))
        return ResultState.Success;
    }

    if (operation.IsResultTypePropertyReference(out var invocation))
    {
      if (invocation.Property.Name is "Success") return ResultState.Success;
      if (invocation.Property.Name is "Error") return ResultState.Error;
    }

    return ResultState.Unknown;
  }

  private static void RegisterDiagnosticsForOperation(IOperation operation, IDictionary<ISymbol, ResultState> state,
    OperationBlockAnalysisContext context) { }

  // need to check the following:
  //
  // assigning 
  //   .Success
  //   .Success()
  //   .Error()
  //   conversion from error type
  //   conversion from success type
  //
  // conversion to value
  //
  // Or
  // OrThrow
  // OrDefault
  //
  // IsError
  // IsError(out)

  private class SymbolResultStatePairEqualityComparer : EqualityComparer<KeyValuePair<ISymbol, ResultState>>
  {
    public new static SymbolResultStatePairEqualityComparer Default { get; } = new();

    public override bool Equals(KeyValuePair<ISymbol, ResultState> x, KeyValuePair<ISymbol, ResultState> y) =>
      x.Key.Equals(y.Key, SymbolEqualityComparer.Default) && x.Value.Equals(y.Value);

    public override int GetHashCode(KeyValuePair<ISymbol, ResultState> obj) =>
      new { symbolHash = SymbolEqualityComparer.Default.GetHashCode(obj.Key), obj.Value }.GetHashCode();
  }
}