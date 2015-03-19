using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using ReturnValueNotUsedAnalyzer;

namespace ReturnValueNotUsedAnalyzer.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        [TestMethod]
        public void VerifyNoDiagnosticOnSimpleString()
        {
            string test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void VerifyNoDiagnosticsWithMethodResultUsed()
        {
            string test = Properties.Resources.functionThatUsesReturnResult;
         
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void VerifyDiagnosticsWhenMethodNotUsed()
        {
            string test = Properties.Resources.functionThatDoesNotUseReturnResult;

            var expected = new DiagnosticResult
            {
                Id = ReturnValueNotUsedAnalyzerAnalyzer.DiagnosticId,
                Message = "Function 'TestMethod' does not return a value.  Please consider if you should handle the return value.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 5, 9)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ReturnValueNotUsedAnalyzerAnalyzer();
        }
    }
}