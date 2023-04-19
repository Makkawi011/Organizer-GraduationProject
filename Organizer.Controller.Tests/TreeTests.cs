using Organizer.Tree;

namespace Organizer.Controller.Tests;

public class TreeTests
{
    [Fact]
    public void BuildFileStructureTree_ReturnsListOfNodes_WhenInputCollectionOfBlocks()
    {
        // Arrange
        string header1 = "ctorHeader();";
        string header2 = "H1();";
        string code = header1 + "{ " + header2 + "{ } }";

        var len = code.Length;

        var blocks = CreateBlockSyntaxes(code);
        List<Node> nodes = CreateNodes(blocks);

        nodes.First().AppendChild(nodes.Last());
        nodes.Last().Value!.Header = CSharpSyntaxTree
            .ParseText(header2)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        var expectedNodes = nodes.ToList();

        // Act
        var actualNodes = blocks.BuildFileStructureTree()!;

        // Assert
        Assert.Equal(expectedNodes.First().Children.Count, actualNodes.First().Children.Count);
        Assert.Equivalent(expectedNodes.Last().Children, actualNodes.Last().Children);
        Assert.Null(actualNodes.First().Parent);
        Assert.Equivalent(expectedNodes.First().Parent, actualNodes.First().Parent);
        Assert.Equivalent(expectedNodes.Last().Value!.Block, actualNodes.Last().Value!.Block);
        Assert.Equivalent(expectedNodes.First().Value!.Header, actualNodes.First().Value!.Header);
    }

    [Fact]
    public void GetRoot_ReturnsRoot_WhenTheInputAllOfNodes()
    {
        // Arrange
        string code = "{ H1() { } }";
        var blocks = CreateBlockSyntaxes(code);
        List<Node> nodes = CreateNodes(blocks);
        nodes.First().AppendChild(nodes.Last());

        var expectedRoot = nodes.First();

        // Act
        var actualRoot = nodes.GetRoot();

        // Assert
        Assert.Equivalent(expectedRoot, actualRoot);
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

    #endregion Helpers
}