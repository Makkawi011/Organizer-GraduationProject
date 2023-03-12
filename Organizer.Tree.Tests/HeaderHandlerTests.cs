using Organizer.Client;

namespace Organizer.Tree.Tests;

public class HeaderHandlerTests
{
    [Fact]
    public void RefactorCreateFolderPaths_ReturnUpdatedCreateFolderInvocs_When3SeqCreatFolderInvocs()
    {
        var headers =GetBlocks("""
                {
                CreateFolder("Path1");
                    {
                        CreateFolder("Path2");
                        {
                            CreateFolder("Path3"); { }
                        }
                    }
                }
                """);

        var invocations = GetInvocations("""
            CreateFolder("Path1");
            CreateFolder("Path2");
            CreateFolder("Path3");
            """)
            .Select(invoc => new InvocationExpressionSyntax[]{invoc});

        var node3 = new Node()
        {
            
            Value = new Value
            {
                Block = headers.ElementAt(3),
                Header = invocations.ElementAt(2)
            }
        };

        var node2 = new Node()
        {
            Children = new List<Node> { node3 },
            Value = new Value
            {
                Block = headers.ElementAt(2),
                Header = invocations.ElementAt(1)
            }
        };
        
        var node1 = new Node()
        {
            Children = new List<Node> { node2, node3 },
            Value = new Value
            {
                Block = headers.ElementAt(1),
                Header =  invocations.First()
            }
        };

        var node0 = new Node
        {
            Children = new List<Node> { node1, node2, node3 },
            Value = new Value
            {
                Block = headers.First(),
                Header = Enumerable.Empty<InvocationExpressionSyntax>()
            },
            Parent = null
        };
        node1.Parent = node0;
        node2.Parent = node1;
        node3.Parent = node2;

        var nodes = new List<Node>(){
            node0,node1,node2,node3
        };

        // Act
        var method = typeof(HeaderHandler)
            .GetMethod("RefactorCreateFolderPaths",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var newInvocations = new List<IEnumerable<InvocationExpressionSyntax>>
        {
            Enumerable.Empty<InvocationExpressionSyntax>()
        };

        for (int i = 0; i < invocations.Count(); i++)
        {
            var newInvocation = (IEnumerable<InvocationExpressionSyntax>?)method!
                .Invoke(null, new object[] { invocations.ElementAt(i), nodes.ElementAt(i).Parent })!;
            nodes.ElementAt(i).Value.Header = newInvocation;

            newInvocations.Add(newInvocation);
        }

        var expectedTherdInvoc = nameof(OrganizerServices.CreateFolder) + "(\"Path1\\\\Path2\\\\Path3\")";
        Assert.Equal(expectedTherdInvoc.ToString(), newInvocations.Last().Last().ToString());
    }

    [Fact]
    public void SetHeaderNode_UpdatedNode_WhenAddHeader()
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

        oldNode.SetHeaderNode(headerInvocation);

        // Assert

        Assert.True(oldNode.Value!.Header == headerInvocation);
    }

    [Fact]
    public void SetHeaderNode_UpdatedNodeByUpdateHeader_WhenAddMultHeader()
    {
        // Arrange

        var supFolderPath = "\"path1\"";
        var createSup = @"CreateFolder("+ supFolderPath + ")";

        var subFolderPath = "\"path2\"";
        var createSub = @"CreateFolder("+ subFolderPath + ")";

        var fullPath = "\"path1\\\\path2\"";
        var expectedInvocation = CSharpSyntaxTree
            .ParseText(nameof(OrganizerServices.CreateFolder)+"("+fullPath+")")
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

                //header => empty 
            }
        };
        root.SetHeaderNode(rootHeaderInvocations);

        var node1 = new Node()
        {
            Parent = null,
            Value = new()
            {
                Block = blockSyntax.ElementAt(1)

                //header => empty  
            }
        };
        root.AppendChild(node1);

        node1.SetHeaderNode(node1HeaderInvocations);

        var node2 = new Node()
        {
            Parent = null,
            Value = new()
            {
                Block = blockSyntax.Last()

                //header => empty
            }
        };
        node1.AppendChild(node2);

        node2.SetHeaderNode(node2HeaderInvocations);

        // Assert
        Assert.Equal(expectedInvocation.ToString(), node2.Value.Header.Single().ToString());
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
