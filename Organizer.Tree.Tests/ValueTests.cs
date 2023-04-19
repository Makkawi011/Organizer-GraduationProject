namespace Organizer.Tree.Tests;

public class ValueTests
{
    [Fact]
    public void Value_Block_Should_Be_Set()
    {
        // Arrange
        var value = new Value();
        var block = CSharpSyntaxTree.ParseText("{ }")
            .GetRoot()
            .DescendantNodes()
            .OfType<BlockSyntax>()
            .First();

        // Act
        value.Block = block;

        // Assert
        Assert.Same(block, value.Block);
    }

    [Fact]
    public void Value_Header_Should_Be_Set()
    {
        // Arrange
        var value = new Value();
        var header = CSharpSyntaxTree.ParseText("H1( );")
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        // Act
        value.Header = header;

        // Assert
        Assert.Equal(header, value.Header);
    }

    [Fact]
    public void IsParentOf_ReturnsTrue_WhenNode1ParentOfNode2()
    {
        // Arrange
        var blocks = CSharpSyntaxTree.ParseText("{{}}")
            .GetRoot()
            .DescendantNodes()
            .OfType<BlockSyntax>();

        List<Node> nodes = new();

        foreach (var block in blocks)
        {
            var node = new Node()
            {
                Value = new Value()
                {
                    Block = block
                }
            };

            nodes.Add(node);
        }

        var parent = nodes.First();
        var child = nodes[1];

        parent.AppendChild(child);

        // Act
        var result = parent.IsParentOf(child);

        // Assert
        Assert.True(result);
    }
}