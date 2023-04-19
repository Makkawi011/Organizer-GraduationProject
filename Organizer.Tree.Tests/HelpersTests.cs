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

    [Fact]
    public void GetParameterValue_WhenInputArgumentSyntax_ReturnTheActualParameters()
    {
        // Arrange
        var code = @"invoc(nameof(parameter))";

        var argumentSyntax =
            CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<ArgumentSyntax>()
            .First();

        // Act
        var actual = argumentSyntax
            .GetParameterValue();

        // Assert
        Assert.Equal("parameter", actual);
    }

    [Fact]
    public void GetName_WhenInputInvocationExpressionSyntax_ReturnTheMethodName()
    {
        // Arrange
        var code = @"MethodName(parameters)";

        var invocation =
            CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .First();

        // Act
        var method = typeof(Helpers)
            .GetMethod("GetName",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var actual = (string?)method!
            .Invoke(null, new[] { invocation });

        // Assert
        Assert.Equal("MethodName", actual);
    }

    [Fact]
    public void IsName_WhenInputInvocationExpressionSyntax_ReturnTheMethodName()
    {
        // Arrange
        var code = @"MethodName(parameters)";

        var invocation =
            CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .First();

        // Act
        var actual = invocation.IsName("MethodName");

        // Assert
        Assert.True(actual);
    }

    #region Helpers

    private static Node? CreatNodeWithDepth(int depth)
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

    #endregion Helpers
}