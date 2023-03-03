using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Organizer.Client;
using Organizer.Services;
using Organizer.Tree;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Organizer.Generator.Services.TypeServices
{
    public static class ContainTypeServcie
    {
        public static void ContainForTypes
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
        {
            if (nodes is null) return;

            foreach ((Node node, string fullTargetPath)
                in from node in nodes
                   let fullTargetPath = GetFullTargetPath(targetPath, node)
                   select (node, fullTargetPath))
            {
                node.Value?
                    .GetPrimaryBlockInvocations()
            }
        }

            IEnumerable<BaseTypeDeclarationSyntax> types , 
        {
            invocations
                .GetSingleParamsOf(nameof(OrganizerServices.ContainType))
                .GetTypesToCreateByNames(types)
                .CreateRequeredTypes(fullTargetPath, context);

            return invocations;
        }

            (this IEnumerable<InvocationExpressionSyntax> invocations,
            IEnumerable<BaseTypeDeclarationSyntax> types,
        {
            var typesInfoToCreate = invocations
                .GetMultParamsOf(nameof(OrganizerServices.ContainTypes));

            var acceptedPatterns = typesInfoToCreate
                .Select(info => info.First());
            
            var ignoredPatterns = typesInfoToCreate
                .Where(info => info.Count() > 1)
                .Select(info => info.ElementAt(1));

            types
                .GetTypesToCreateByPatterns(ignoredPatterns , acceptedPatterns)
                .CreateRequeredTypes(fullTargetPath, context);

        }

        private static void CreateRequeredTypes
            (this IEnumerable<BaseTypeDeclarationSyntax> typesToCreate,
            string fullTargetPath,
            GeneratorExecutionContext context)
        {
            foreach (var (type, typePath)
                in from type in typesToCreate
                   let typePath = Path.Combine(fullTargetPath, type.Identifier.Text + ".g.cs")
                   select (type.ToString(), typePath))
            {
                context.AddSource(typePath, source: type);
            }
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

        private static IEnumerable<BaseTypeDeclarationSyntax> GetTypesToCreateByNames
            (this IEnumerable<string> typesNameToCreate,
            IEnumerable<BaseTypeDeclarationSyntax> types)
            => (from type in types
                join typeName in typesNameToCreate
                on type.Identifier.Text.ToString() equals typeName
                select type) as IEnumerable<BaseTypeDeclarationSyntax>;

        private static IEnumerable<BaseTypeDeclarationSyntax> GetTypesToCreateByPatterns
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            IEnumerable<string> ignoredPatterns,
            IEnumerable<string> acceptedPatterns) 
            => (from type in types

                from ignorePattern in ignoredPatterns
                where !Regex.IsMatch(type.Identifier.Text.ToString(), ignorePattern)

                from acceptPattern in acceptedPatterns
                where Regex.IsMatch(type.Identifier.Text.ToString(), acceptPattern)

                select type) as IEnumerable<BaseTypeDeclarationSyntax>;
    }
}
