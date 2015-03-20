using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Formatting;

namespace ReturnValueNotUsedAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReturnValueNotUsedAnalyzerCodeFixProvider)), Shared]
    public class ReturnValueNotUsedAnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ReturnValueNotUsedAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);


            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create("Add Return Value", c => AddReturnValue(context.Document, declaration, c)),
                diagnostic);
        }

        private async Task<Document> AddReturnValue(Document document, InvocationExpressionSyntax functionSytax, CancellationToken cancellationToken)
        {
            // Get the symbol representing the type to be renamed.
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            IMethodSymbol typeSymbol = semanticModel.GetSymbolInfo(functionSytax).Symbol as IMethodSymbol;

            var root = await document.GetSyntaxRootAsync();

            var returnValue =
                         SyntaxFactory.LocalDeclarationStatement(
                            SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName(
                                    @"var"))
                            .WithVariables(
                                SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    SyntaxFactory.VariableDeclarator(
                                        SyntaxFactory.Identifier(
                                            @"s"))
                                    .WithInitializer(
                                        SyntaxFactory.EqualsValueClause(functionSytax))))).WithAdditionalAnnotations(Formatter.Annotation);

            var parent = functionSytax.Parent;

            // Must replace on same level in tree.
            var newRootNode = root.ReplaceNode(parent, returnValue);
            var newDocument = document.WithSyntaxRoot(newRootNode);
            
            // Return the new solution with the now-uppercase type name.
            return newDocument;
        }


    }
}