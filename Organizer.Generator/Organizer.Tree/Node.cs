using System.Collections.Generic;
using System.Linq;

namespace Organizer.Tree
{
    public sealed class Node
    {
        public Value Value { get; set; }
        public List<Node> Children { get; set; } = Enumerable.Empty<Node>().ToList();
        public Node Parent { get; set; }


        public bool IsLeaf => Children.Count == 0;

        internal void AppendChild(Node child)
        {
            Children.Insert(0, child);
            child.Parent = this;
        }

        internal bool IsParentOf(Node child) 
            => Value.Block.SpanStart < child.Value.Block.SpanStart
            && Value.Block.Span.End > child.Value.Block.Span.End;
    }
}



