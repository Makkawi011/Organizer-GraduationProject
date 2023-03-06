using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Organizer.Tree;

namespace Organizer.Controller
{
    public static class Tree
    {
        public static List<Node> BuildFileStructureTree(this IEnumerable<BlockSyntax> blocks)
            => Builder.BuildTree(blocks);

        public static Node GetRoot(this List<Node> nodes)
            => nodes.FirstOrDefault(n => n.Parent is null);
    }
}
