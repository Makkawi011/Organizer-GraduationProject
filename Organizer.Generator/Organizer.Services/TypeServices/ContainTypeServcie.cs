using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Organizer.Client;
using Organizer.Services;
using Organizer.Tree;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Organizer.Generator.Services.TypeServices
{
    public static class ContainTypeServcie
    {
        public static void ContainForTypes
             (this IEnumerable<BaseTypeDeclarationSyntax> types,
             List<Node> nodes,
             string targetPath)
        {
            if (nodes is null) return;

            string paths = string.Empty;
            int x = 0;

            foreach ((Node node, string fullTargetPath)
                in from node in nodes
                   let fullTargetPath = GetFullTargetPath(targetPath, node)
                   select (node, fullTargetPath))
            {
                paths += x++ + "=>" + fullTargetPath;
                if (x == 1) continue;

                node.Value
                    .GetPrimaryBlockInvocations()
                    .ContainForTypesByName(types, fullTargetPath)
                    .ContainForTypesByPattern(types, fullTargetPath);

            }

        }

        public static IEnumerable<InvocationExpressionSyntax> ContainForTypesByName
           (this IEnumerable<InvocationExpressionSyntax> invocations,
           IEnumerable<BaseTypeDeclarationSyntax> types,
           string fullTargetPath)
        { 
            invocations
                .GetSingleParamsOf(nameof(OrganizerServices.ContainType))
                .GetTypesToCreateByNames(types)
                .CreateRequeredTypes(fullTargetPath);


            return invocations;
        }

        public static void ContainForTypesByPattern
           (this IEnumerable<InvocationExpressionSyntax> invocations,
           IEnumerable<BaseTypeDeclarationSyntax> types,
           string fullTargetPath)
        {
            var typesInfoToCreate = invocations
                .GetMultParamsOf(nameof(OrganizerServices.ContainTypes));

            var acceptedPatterns = typesInfoToCreate
                .Select(info => info.First());

            var ignoredPatterns = typesInfoToCreate
                .Where(info => info.Count() > 1)
                .Select(info => info.ElementAt(1));


            //throw new System.Exception
            //    ((from type in types
            //      from acceptPattern in acceptedPatterns
            //      where Regex.IsMatch(type.Identifier.Text.ToString(), acceptPattern)
                  
            //      from ignorePattern in ignoredPatterns
            //      where !Regex.IsMatch(type.Identifier.Text.ToString(), ignorePattern)
            //      select type).Count().ToString()) ;

            types
                .GetTypesToCreateByPatterns(ignoredPatterns , acceptedPatterns)
                .CreateRequeredTypes(fullTargetPath);

        }

        private static void CreateRequeredTypes
            (this IEnumerable<BaseTypeDeclarationSyntax> typesToCreate,
            string fullTargetPath)
        {
            foreach (var (type, typePath)
                in from type in typesToCreate
                   let typePath = Path
                   .Combine(fullTargetPath, type.Identifier.Text + ".g.cs")
                   .Replace("\\\\", "\\")
                   .Replace("\\", "\\\\")
            select (type.ToString(), typePath))
            {
                File.WriteAllText(typePath, type.ToString());
            }
        }

        private static string GetFullTargetPath(string targetPath, Node node)
        {
            var folderPath = node.Value?.Header?
                .GetSingleParamsOf(nameof(OrganizerServices.CreateFolder))
                .LastOrDefault();
            
            return folderPath is null ? targetPath : 
                Path.Combine(targetPath, folderPath)
                .Replace("\\\\", "\\")
                .Replace("\\", "\\\\"); ;
        }
        private static IEnumerable<InvocationExpressionSyntax> GetPrimaryBlockInvocations
            (this Value value)
        {

            var invocations = new string(
                value
                .Block
                .Statements
                .ToString()
                .Split('}')
                .SelectMany(s => s.TakeWhile(c => c != '{'))
                .ToArray());

            return CSharpSyntaxTree
                .ParseText(invocations)
                .GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>();
        }

        private static IEnumerable<BaseTypeDeclarationSyntax> GetTypesToCreateByNames
            (this IEnumerable<string> typesNameToCreate,
            IEnumerable<BaseTypeDeclarationSyntax> types)
            => from type in types
               join typeName in typesNameToCreate
               on type.Identifier.Text.ToString() equals typeName
               select type;

        private static IEnumerable<BaseTypeDeclarationSyntax> GetTypesToCreateByPatterns
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            IEnumerable<string> ignoredPatterns,
            IEnumerable<string> acceptedPatterns)
            => types
            .Where(type =>
                acceptedPatterns.Any(acceptPattern
                    => Regex.IsMatch(type.Identifier.Text.ToString(), acceptPattern)))
            .Where(type =>
                !ignoredPatterns.Any(ignorePattern
                    => Regex.IsMatch(type.Identifier.Text.ToString(), ignorePattern)));
    }
}
