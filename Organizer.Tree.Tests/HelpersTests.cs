namespace Organizer.Tree.Tests;

public class HelpersTests
{

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void MaxDepthStartingFrom_CalculateTreeDepth(int depth)
    {
        // Arrange
        var node = CreatNodeWithDepth(depth);

        // Act
        int actualDepth = Helpers.MaxDepthStartingFrom(node);

        // Assert
        Assert.Equal(depth, actualDepth);
    }



    #region Helpers
    static Node? CreatNodeWithDepth(int depth)
    {
        if (depth <= 0) return null;
        if (depth == 1) return new();

        var nodes = Enumerable
            .Range(1, depth)
            .Select(i => new Node())
            .ToList();

        for (int i = 0; i < nodes.Count - 1; i++)
            nodes[i].AppendChild(nodes[i + 1]);

        return nodes.First();
    }
    #endregion

}
