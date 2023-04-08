using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Organizer.Tree
{
    public static class Helpers
    {
        internal static int MaxDepthStartingFrom(Node node)
        {
            if (node is null) return 0;
            if (node.IsLeaf) return 1;

            int depth = 0;
            foreach (var child in node.Children)
            {
                int childDepth = MaxDepthStartingFrom(child);
                depth = Math.Max(depth, childDepth);
            }

            return depth + 1;
        }
        public static bool IsName(this InvocationExpressionSyntax invocation, string InvocationName)
            => invocation.GetName().Equals(InvocationName);
        private static string GetName(this InvocationExpressionSyntax invocation) 
            => ((IdentifierNameSyntax)invocation.Expression)
                .Identifier
                .ValueText
                .ToString();


        public static string GetParameterValue(this ArgumentSyntax arg)
            => arg.ToString()
                .Replace("\"", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("nameof(", string.Empty)
                .Replace(")", string.Empty);
    }
}
