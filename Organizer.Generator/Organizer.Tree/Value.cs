using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Organizer.Tree
{
    public sealed class Value
    {
        public BlockSyntax Block { get; set; }
        public IEnumerable<InvocationExpressionSyntax> Header { get; set; }
    }
}