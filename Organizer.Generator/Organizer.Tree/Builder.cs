using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

using static Organizer.Tree.HeaderHandler;

namespace Organizer.Tree
{
    public sealed class Builder
    {
        public static List<Node> BuildTree(IEnumerable<BlockSyntax> blocks)
            => blocks is null ? null : RefactorNodeInformations(BuildEdges(BuildNodes(blocks)));


        private static List<Node> BuildNodes(IEnumerable<BlockSyntax> blocks)
        {
            List<Node> nodes = new List<Node>();

            foreach (var block in blocks)
            {
                var node = new Node()
                {
                    Value = new Value()
                    {
                        Block = block,
                        Header = Enumerable.Empty<InvocationExpressionSyntax>()
                    }
                };

                nodes.Insert(0, node);
            }

            return nodes;
        }
        private static List<Node> BuildEdges(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                nodes
                .FirstOrDefault(parent => parent.IsParentOf(node))
                ?.AppendChild(node);
            }

            nodes.Reverse();

            return nodes;
        }
        private static List<Node> RefactorNodeInformations(List<Node> nodes)
        {
            var root = nodes.FirstOrDefault();
            var code = root.Value.Block.SyntaxTree.ToString();

            var depth = Helpers.MaxDepthStartingFrom(root);
            for (int n = 0; n < depth - 1; n++)
            {
                var parent = nodes[n].IsLeaf ? nodes[n].Parent : nodes[n];
                if (parent is null) continue;

                var childrens = parent.Children;

                //update first childe 
                var firstChild = childrens.First();
                
                var headerFirstChild = GetHeaderNode(
                        code,
                        parent.Value.Block.SpanStart,
                        firstChild.Value.Block.SpanStart);

                var updatedFirstChild = firstChild.SetHeaderNode(headerFirstChild);

                var indexOfFirstChild = nodes.IndexOf(firstChild);
                nodes.RemoveAt(indexOfFirstChild);
                nodes.Insert(indexOfFirstChild, updatedFirstChild);

                //update rest of childrens ...
                for (int i = 1; i < childrens.Count; i++)
                {
                    //priveus child 
                    var priveusChild = childrens[i - 1];
                    //update current child
                    var currentChild = childrens[i];


                    var headerCurrentChild = GetHeaderNode(
                        code,
                        priveusChild.Value.Block.Span.End,
                        currentChild.Value.Block.SpanStart);


                    var updatedCurrentChild = currentChild.SetHeaderNode(headerCurrentChild);

                    childrens.RemoveAt(i);
                    childrens.Insert(i, updatedCurrentChild);


                    var indexOfCurrentChild = nodes.IndexOf(currentChild);
                    nodes.RemoveAt(indexOfCurrentChild);
                    nodes.Insert(indexOfCurrentChild, updatedCurrentChild);
                }

            }
            return nodes;
        }
    }
}
