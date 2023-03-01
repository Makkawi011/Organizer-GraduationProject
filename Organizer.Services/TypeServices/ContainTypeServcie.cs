using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Organizer.Client;
using Organizer.Services;
using Organizer.Tree;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Organizer.Generator.Services.TypeServices
{
    public static class ContainTypeServcie
    {
        public static void CreateForTypes
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            List<Node> nodes ,
            string targetPath)
        {
            foreach ((Node node, string fullTargetPath)
                in from node in nodes
                   let fullTargetPath = GetFullTargetPath(targetPath, node)
                   select (node, fullTargetPath))
            {
                node.Value?
                    .GetPrimaryBlockInvocations()
                    .CreateForTypesByName(types, fullTargetPath)
                    .CreateForTypesByPattern(types, fullTargetPath);
            }
        }

        public static IEnumerable<InvocationExpressionSyntax> CreateForTypesByName
            (this IEnumerable<InvocationExpressionSyntax> invocations,
            IEnumerable<BaseTypeDeclarationSyntax> types , 
            string fullTargetPath)
        {
            return invocations;
        }

        public static void CreateForTypesByPattern
            (this IEnumerable<InvocationExpressionSyntax> invocations,
            IEnumerable<BaseTypeDeclarationSyntax> types,
            string fullTargetPath)
        {

        }

        private static string GetFullTargetPath(string targetPath, Node node)
        {
            var folderPath = node.Value?.Header?
                .GetSingleParamsOf(nameof(OrganizerServices.CreateFolder))
                .Last();

            return Path.Combine(targetPath, folderPath);
        }
        private static IEnumerable<InvocationExpressionSyntax> GetPrimaryBlockInvocations
            (this Value value)
        {
            var invocations = value
                .Block!
                .Statements
                .ToString()
                .Split('}')
                .SelectMany(s => s.TakeWhile(c => c != '{'))
                .ToArray();

            return CSharpSyntaxTree
                .ParseText(new string(invocations))
                .GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>();
        }
    }
}
