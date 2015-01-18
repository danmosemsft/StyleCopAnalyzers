﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1005SingleLineCommentsMustBeginWithSingleSpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the comment begins with a single space. If the comment is
    /// being used to comment out a line of code, ensure that the comment begins with four forward slashes, in which
    /// case the leading space can be omitted.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1005CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1005CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1005SingleLineCommentsMustBeginWithSingleSpace.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1005SingleLineCommentsMustBeginWithSingleSpace.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxTrivia trivia = root.FindTrivia(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
                if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    continue;

                string text = trivia.ToFullString();
                if (!text.StartsWith("//"))
                    continue;

                string correctedText = "// " + text.Substring(2);
                SyntaxTrivia corrected = SyntaxFactory.Comment(correctedText).WithoutFormatting();
                Document updatedDocument = context.Document.WithSyntaxRoot(root.ReplaceTrivia(trivia, corrected));
                context.RegisterFix(CodeAction.Create("Insert space", updatedDocument), diagnostic);
            }
        }
    }
}
