using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

using static Organizer.Tree.HeaderHandler;

namespace Organizer.Tree
{
    public sealed class Builder
    {
        public static List<Node> BuildTree(IEnumerable<BlockSyntax> blocks)
            => blocks is null ? null : RefactorInfos(BuildEdges(BuildNodesByDescending(blocks)));



        private static List<Node> BuildNodesByDescending(IEnumerable<BlockSyntax> blocks)
        {
            var nodes = new List<Node>();

            for (int i = blocks.Count() - 1; i >= 0; i--)
            {
                var node = new Node()
                {
                    Value = new Value()
                    {
                        Block = blocks.ElementAt(i)
                    }
                };

                nodes.Add(node);
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
        private static List<Node> RefactorInfos(List<Node> nodes) 
            => AddUpdatedHeaders(nodes);


        #region Add Updated Headers Functions
        private static List<Node> AddUpdatedHeaders(List<Node> nodes)
        {
            var root = nodes.FirstOrDefault(n=> n.Parent is null);
            var code = root.Value.Block.SyntaxTree.ToString();

            var depth = Helpers.MaxDepthStartingFrom(root);
            for (int n = 0; n < depth - 1; n++)
            {
                var parent = nodes[n].IsLeaf ? nodes[n].Parent : nodes[n];
                if (parent is null) continue;

                AddUpdatedHeadersToChildsOf(parent, code);

            }
            return nodes;
        }

        private static void AddUpdatedHeadersToChildsOf(Node parent, string code)
        {
            //update first childe 
            AddUpdatedHeaderToFirstChildOf(parent, code);

            //update rest of childrens ...
            AddUpdatedHeadersToRestChildsOf(parent.Children, code);
        }
        private static void AddUpdatedHeaderToFirstChildOf(Node parent , string code)
        {
            //update first childe 
            var firstChild = parent.Children.First();

            var headerFirstChild = GetHeaderNode(
                    code,
                    parent.Value.Block.SpanStart,
                    firstChild.Value.Block.SpanStart);

            firstChild.SetHeaderNode(headerFirstChild);
        }
        private static void AddUpdatedHeadersToRestChildsOf(List<Node> childrens ,string code)
        {
            //first Child we handled resently ...
            //update rest of childrens ...
            for (int i = 1; i < childrens.Count; i++)
            {
                //previous child
                var previousChild = childrens[i - 1];
                //update current child
                var currentChild = childrens[i];


                var headerCurrentChild = GetHeaderNode(
                    code,
                    previousChild.Value.Block.Span.End,
                    currentChild.Value.Block.SpanStart);


                currentChild.SetHeaderNode(headerCurrentChild);

            }
        }

        #endregion
    }
}
