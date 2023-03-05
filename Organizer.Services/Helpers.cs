using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Organizer.Tree;

namespace Organizer.Services
{
    internal static class Helpers
    {
        internal static IEnumerable<InvocationExpressionSyntax>? GetInvocations(this ConstructorDeclarationSyntax? constructor)
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
                .SingleOrDefault();
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
    }
}
