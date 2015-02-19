using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReturnValueNotUsedAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnValueNotUsedAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ReturnValueNotUsed";
        internal const string Title = "Return value of function is not used.";
        internal const string MessageFormat = "Function '{0}' does not return a value.  Please consider if you should handle the return value.";
        internal const string Category = "Naming";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var symbol = context.SemanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;
            if (symbol == null || symbol.ReturnsVoid)
            {
                return;
            }

            // TODO make a recusive call up tree to see if we find the value being used.
            var parentNode = node.Parent as EqualsValueClauseSyntax;
            if (parentNode == null)
            {
                var diagnostic = Diagnostic.Create(Rule, node.GetLocation(), symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
