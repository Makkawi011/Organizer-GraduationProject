namespace Organizer.Tree.Tests;

public class BuilderTests
{
    [Fact]
    public void BuildNodes_ReturnsCollectionOfNodes_WhenInputCollectionOfBlocks()
    {
        // Arrange
        var code = "H1(){ H2() { } }";

        var blocks = CreateBlockSyntaxes(code);
        var expectedNodes = CreateNodeDescending(blocks);

        // Act

        var builder =  new Builder();
        var method = typeof(Builder)
            .GetMethod("BuildNodes", BindingFlags.NonPublic | BindingFlags.Static);
        var actualNodes = (List<Node>?) method!.Invoke(builder, new[] { blocks });

        // Assert

        Assert.Equivalent(expectedNodes , actualNodes);
    }

    [Fact]
    public void BuildEdges_ReturnsCollectionOfConnectedNodes_WhenInputListOfNodes()
    {
        // Arrange
        var code = "H1(){ H2() { } }";
        var blocks = CreateBlockSyntaxes(code);
        var nodes = CreateNodeDescending(blocks);

        nodes[0].AppendChild(nodes[1]);

        var expectedNodes = nodes.ToList();
        expectedNodes.Reverse();

        // Act

        var builder = new Builder();
        var method = typeof(Builder)
            .GetMethod("BuildEdges", BindingFlags.NonPublic | BindingFlags.Static);
        var actualNodes = (List<Node>?)method!.Invoke(builder, new[] { nodes });

        // Assert

        Assert.Equivalent(expectedNodes, actualNodes);
    }

    [Fact]
    public void RefactorInfos_ReturnsCollectionOfConnectedNodesWithFillHeadersInValueOfNodes()
    {
        // Arrange
        string header1 = "H0()";
        string header2 = "H1()";
        string code = header1 + "{ "+ header2 + "{ } }";
        var blocks = CreateBlockSyntaxes(code);
        List<Node> nodes = CreateNodes(blocks);

        nodes.First().Parent = nodes.Last();
        nodes.Last().Value!.Header = GetInvocs(header2);
        nodes.First().Value!.Header = GetInvocs(header1);
        nodes.Reverse();

        var expectedNodes = nodes.ToList();

        // Act

        var builder = new Builder();
        var method = typeof(Builder)
            .GetMethod("RefactorInfos", BindingFlags.NonPublic | BindingFlags.Static);
        var actualNodes = (List<Node>?)method!.Invoke(builder, new[] { nodes });

        // Assert

        Assert.Equivalent(expectedNodes, actualNodes);
    }
    #region Helpers
    private static IEnumerable<BlockSyntax> CreateBlockSyntaxes(string code)
    {
        return CSharpSyntaxTree
             .ParseText(code)
             .GetRoot()
             .DescendantNodes()
             .OfType<BlockSyntax>();
    }
    private static List<Node> CreateNodeDescending(IEnumerable<BlockSyntax> blocks)
    {
        var nodes = CreateNodes(blocks);
        nodes.Reverse();
        return nodes;
    }
    private static List<Node> CreateNodes(IEnumerable<BlockSyntax> blocks)
    {
        return Enumerable.Range(0, blocks.Count())
            .Select(i =>
            new Node()
            {
                Value = new Value()
                {
                    Block = blocks.ElementAt(i),
                    Header = Enumerable.Empty<InvocationExpressionSyntax>()
                }
            })
            .ToList();
    }
    private static IEnumerable<InvocationExpressionSyntax> GetInvocs(string header)
    {
        return CSharpSyntaxTree
             .ParseText(header)
             .GetRoot()
             .DescendantNodes()
             .OfType<InvocationExpressionSyntax>();
    }
    #endregion
}
