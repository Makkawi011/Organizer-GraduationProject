namespace Organizer.Tree.Tests;

public class NodeTests
{
    [Fact]
    public void IsLeaf_ReturnTrue_WhereNoChildren()
    {
        var node = new Node();
        Assert.True(node.IsLeaf);
    }

    [Fact]
    public void AddChild_Should_Add_Child_Node_To_Parent()
    {
        // Arrange
        var parent = new Node();
        var child = new Node();

        // Act
        parent.AppendChild(child);

        // Assert
        Assert.Same(parent, child.Parent);
        Assert.Contains(child, parent.Children);
    }
}
