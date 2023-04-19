using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Organizer.Tree;

namespace Organizer.Services
{
    internal static class Helpers
    {
        internal static IEnumerable<InvocationExpressionSyntax> GetInvocations(this ConstructorDeclarationSyntax constructor)
        {
            return constructor?
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>();
        }

        internal static string GetTypeName(this ArgumentSyntax arg)
            => arg.GetParameterValue();

        internal static BaseTypeDeclarationSyntax ConvertToBaseTypeDeclarationSyntax(string type)
            => CSharpSyntaxTree.ParseText(type)
                .GetRoot()
                .DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>()
                .ElementAtOrDefault(0);

        internal static IEnumerable<InvocationExpressionSyntax> GetInvocationsByName
            (this IEnumerable<InvocationExpressionSyntax> invocations, string organizerServiceName)
            => invocations.Where(invoc => invoc.IsName(organizerServiceName));

        internal static IEnumerable<string> GetSingleParamsOf
            (this IEnumerable<InvocationExpressionSyntax> invocations,
            string organizerServiceName)
        {
            return invocations
                .GetInvocationsByName(organizerServiceName)
                .SelectMany(invoc => invoc.ArgumentList.Arguments)
                .Select(arg => arg.GetParameterValue());
        }

        internal static IEnumerable<IEnumerable<string>> GetMultParamsOf
            (this IEnumerable<InvocationExpressionSyntax> invocations,
            string organizerServiceName)
        {
            return invocations
                .GetInvocationsByName(organizerServiceName)
                .Select(invoc => invoc.ArgumentList.Arguments)
                .Select(args => args.Select(arg => arg.GetParameterValue()));
        }

        /// <summary>
        /// convert BaseTypeDeclarationSyntax to string
        /// then add to this string
        /// the namespace declaration and the using directives
        /// </summary>
        /// <param name="type">ClassDeclarationSyntax or StructDeclarationSyntax or EnumDeclarationSyntax</param>
        /// <returns>string contain : using directives then the namespace declaration have the input type declaration</returns>
        internal static string RefactoreToString(this BaseTypeDeclarationSyntax type)
        {
            var nodes = type
                .Parent
                .SyntaxTree
                .GetRoot()
                .DescendantNodes();

            var namespaceName = nodes
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()?
                .Name;

            var usings = string.Join("\n",
                nodes
                .OfType<UsingDirectiveSyntax>()
                .Select(@using => @using.ToString()));

            return namespaceName is null
                ? Normailze(string.Concat(usings, type))
                : Normailze(string.Concat(usings, " namespace ", namespaceName, " {", type, " }"));
        }

        internal static string Normailze(string code)
            => CSharpSyntaxTree.ParseText(code)
                .GetRoot()
                .NormalizeWhitespace()
                .ToString();
    }
}