﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.ReadabilityRules.SA1129DoNotUseDefaultValueTypeConstructor,
    StyleCop.Analyzers.ReadabilityRules.SA1129CodeFixProvider>;

    public class SA1129CSharp9UnitTests : SA1129CSharp8UnitTests
    {
        /// <summary>
        /// Verifies that target type new expressions for value types will generate diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyValueTypeWithTargetTypeNewAsync()
        {
            var testCode = @"struct S
{
    internal static S F()
    {
        S s = {|#0:new()|};
        return s;
    }
}
";

            var fixedTestCode = @"struct S
{
    internal static S F()
    {
        S s = default;
        return s;
    }
}
";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
