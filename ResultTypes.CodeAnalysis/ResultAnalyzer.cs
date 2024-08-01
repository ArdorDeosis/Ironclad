using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
}