using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System;
using System.Linq;
using Organizer.Client;
using System.IO;

namespace Organizer.Tree
{
    internal static class HeaderHandler
    {
        internal static IEnumerable<InvocationExpressionSyntax> GetHeaderNode(string code, int start, int end)
        {
            if (end < start)
                throw new ArgumentException($"{nameof(start)} = {start} <  {nameof(end)} = {end} , {nameof(code)} = {code}");
            var len = end - start - 1;
            var header = code.Substring(start, len);
            return CSharpSyntaxTree.ParseText(header).GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>();
        }

        internal static void SetHeaderNode(this Node node, IEnumerable<InvocationExpressionSyntax> orginalHeader)
        {
            //header is new information
            node.Value.Header = RefactorCreateFolderPaths(orginalHeader, node.Parent);
        }

        private static IEnumerable<InvocationExpressionSyntax> RefactorCreateFolderPaths(IEnumerable<InvocationExpressionSyntax> orginalHeader, Node parent)
        {
            if (parent is null)
                return orginalHeader;
            var lastParentCreatFolderInvoc = parent.Value.Header?.LastOrDefault(invocation => invocation.IsName(nameof(OrganizerServices.CreateFolder)));
            if (lastParentCreatFolderInvoc is null)
                return orginalHeader;
            var invocations = new List<InvocationExpressionSyntax>();
            foreach (var invocation in orginalHeader)
            {
                if (!invocation.IsName(nameof(OrganizerServices.CreateFolder)))
                    invocations.Add(invocation);
                else
                {
                    var parentFolderName = lastParentCreatFolderInvoc.ArgumentList.Arguments.Single().GetParameterValue();
                    var currentFolderName = invocation.ArgumentList.Arguments.Single().GetParameterValue();
                    var path = Path.Combine(parentFolderName, currentFolderName).Replace("\\\\", "\\").Replace("\\", "\\\\");
                    var newCreateFolderInvocation = nameof(OrganizerServices.CreateFolder) + "(\"" + path + "\");";
                    var newInvoc = CSharpSyntaxTree.ParseText(newCreateFolderInvocation).GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().Single();
                    invocations.Add(newInvoc);
                }
            }

            return invocations;
        }
    }
}