using System;

using Xunit;

using static System.Reflection.Metadata.BlobBuilder;

namespace Organizer.Tree.Tests;

public class HeaderHandlerTests
{

    [Fact]
    public void RefactorCreateFolderPaths_ReturnsUpdatedNode_WhenAddHeader()
    {
        // Arrange
        var header = @"CreateFolder(""path1"")";
        string block = header + "{ Contents }";
        BlockSyntax blockSyntax = CSharpSyntaxTree.ParseText(block)
            .GetRoot()
            .DescendantNodes()
            .OfType<BlockSyntax>()
            .First();

        var oldNode = new Node()
        {
            Parent = null,
            Children = Enumerable.Empty<Node>().ToList(),
            Value = new()
            {
                Block = blockSyntax

                //header => null  by defult 
            }
        };

        var headerInvocation = CSharpSyntaxTree
            .ParseText(header)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();
        // Act

        var newNode = oldNode.SetHeaderNode(headerInvocation);

        // Assert

        Assert.True(newNode.Value!.Header == headerInvocation);
    }

    [Fact]
    public void RefactorCreateFolderPaths_ReturnsUpdatedNode_WhenAddMultHeader()
    {
        // Arrange

        var supFolderPath = "\"path1\"";
        var createSup = @"CreateFolder("+ supFolderPath + ")";

        var subFolderPath = "\"path2\"";
        var createSub = @"CreateFolder("+ subFolderPath + ")";

        var fullPath = "path1\\path2";

        var expectedInvocation = CSharpSyntaxTree
            .ParseText(nameof(Organizer.Client.OrganizerServices.CreateFolder)+"(\""+fullPath+"\")")
            .GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>()
            .Single();

        string blocks = "ctor{"+ createSup + "{" + createSub +"{ } } }";
        var blockSyntax = GetBlocks(blocks); ;
        var rootHeaderInvocations = GetInvocations("Ctor");
        var node1HeaderInvocations = GetInvocations(createSup);
        var node2HeaderInvocations = GetInvocations(createSub);

        var root = new Node()
        {
            Parent = null,
            Value = new()
            {
                Block = blockSyntax.ElementAt(0)

                //header => null  by defult 
            }
        };
        var newRoot = root.SetHeaderNode(rootHeaderInvocations);

        var node1 = new Node()
        {
            Parent = null,
            Value = new()
            {
                Block = blockSyntax.ElementAt(1)

                //header => null  by defult 
            }
        };
        newRoot.AppendChild(node1);

        var newNode1 = node1.SetHeaderNode(node1HeaderInvocations);
        newRoot.AppendChild(newNode1);

        var node2 = new Node()
        {
            Parent = null,
            Value = new()
            {
                Block = blockSyntax.Last()

                //header => null  by defult 
            }
        };
        newNode1.AppendChild(node2);

        var newNode2 = node2.SetHeaderNode(node2HeaderInvocations);

        // Assert
        Assert.Equal(expectedInvocation.ToString(), newNode2.Value.Header.Single().ToString());
    }
    [Fact]
    public void GetNodeHeader_ReturnsNull_WhenNoInvocationsInHeader()
    {
        // Arrange

        string block = null + "{ Contents }"; // Header => null
        int start = 0;
        int end = block.IndexOf('}');

        // Act

        var actualHeader = HeaderHandler.GetHeaderNode(block, start, end);

        // Assert

        Assert.Empty(actualHeader);
    }
    [Fact]
    public void GetNodeHeader_ReturnsInvocationsAsString_WhenInvocationsInHeader()
    {
        // Arrange

        string header = "Call1(); Call2();";
        string block = header + "{ Contents }";
        int start = 0;
        int end = block.IndexOf('}');

        var expectedHeader = CSharpSyntaxTree
            .ParseText(header)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        // Act

        var actualHeader = HeaderHandler.GetHeaderNode(block, start, end);

        // Assert

        Assert.Equivalent(expectedHeader.Select(invo => invo.ToString()), actualHeader.Select(invo => invo.ToString()));
    }

    #region Helpers
    private static IEnumerable<BlockSyntax> GetBlocks(string code) 
        => CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<BlockSyntax>();
    private static IEnumerable<InvocationExpressionSyntax> GetInvocations(string code)
    => CSharpSyntaxTree.ParseText(code)
        .GetRoot()
        .DescendantNodes()
        .OfType<InvocationExpressionSyntax>();

    #endregion
}
