using Organizer.Tree;

namespace Organizer.Controller.Tests;

public class RoslynTests
{
    [Fact]
    public void GetOrganizerConstructor_ReturnsCtor_WhenTheInputClass()
    {
        // Arrange
        var expectedOrganizerCtor = "public Organizer() { }";
        var organizerClss = CSharpSyntaxTree
            .ParseText(@"class Organizer : OrganizerServices {"+ expectedOrganizerCtor +"}")
            .GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First();

        // Act
        var actualOrganizerCtor = organizerClss.GetOrganizerConstructor()!;

        // Assert

        Assert.Equal(expectedOrganizerCtor, actualOrganizerCtor.ToString());
    }

    [Fact]
    public void GetOrganizerConstructor_ReturnsCtor_WhenTheInputNode()
    {
        // Arrange
        var expectedOrganizerCtor = "public Organizer() { }";
        var organizerClss = CSharpSyntaxTree
            .ParseText(@"class Organizer : OrganizerServices {" + expectedOrganizerCtor + "}")
            .GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First();
        var node = CreateNode(organizerClss);

        // Act
        var actualOrganizerCtor = node.GetOrganizerConstructor()!;

        // Assert

        Assert.Equal(expectedOrganizerCtor, actualOrganizerCtor.ToString());
    }
    
    [Fact]
    public void GetBlockSyntaxes_ReturnsBlockSyntaxCollection_WhenTheInputOrganizerCtor()
    {
        // Arrange
        var organizerCtor = CSharpSyntaxTree
            .ParseText(@"class Organizer : OrganizerServices { public Organizer() { block1 } }")
            .GetRoot()
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .First();

        // Act
        var blockSyntaxes = organizerCtor.GetBlockSyntaxes()!;

        // Assert

        Assert.True(blockSyntaxes.Count() == 1);
        Assert.Equal("{ block1 }", blockSyntaxes.Single().ToString());
    }

    [Fact]
    public void GetClasses_ReturnsClassesDeclarationSynatxCollection_WhenTheInputSyntaxTree()
    {
        // Arrange
        var tree = CSharpSyntaxTree
            .ParseText(@"
            class Organizer : OrganizerServices {}
            class clss1 {}
            class clss2 {}");

        var expectedClasses = tree
            .GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>();

        // Act

        var method = typeof(Roslyn)
            .GetMethod("GetClasses",
            BindingFlags.NonPublic |
            BindingFlags.Static,
            new Type[] { typeof(SyntaxTree) });

        var actualClasses = (IEnumerable<ClassDeclarationSyntax>?)method!
            .Invoke(null, new[] { tree });

        // Assert

        Assert.Equivalent(expectedClasses, actualClasses);
    }
    [Fact]
    public void GetClasses_ReturnsClassesDeclarationSynatxCollection_WhenTheInputCollectionOfSyntaxTree()
    {
        // Arrange
        var tree1 = CSharpSyntaxTree.ParseText("class Organizer : OrganizerServices {}");
        var tree2 = CSharpSyntaxTree.ParseText("class clss1 {}");
        var tree3 = CSharpSyntaxTree.ParseText("class clss2 {}");
        var trees = new[] { tree1, tree2, tree3 };


        var expectedClasses = new[] 
        {
            tree1.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>(),
            tree2.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>(),
            tree3.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
        }
        .SelectMany(tree => tree);

        // Act

        var actualClasses = trees.GetClasses();

        // Assert

        Assert.Equivalent(expectedClasses, actualClasses);
    }


    #region Helpers
    private static Node CreateNode(ClassDeclarationSyntax classDeclaration)
    {
        var block =
            classDeclaration
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .First()
            .DescendantNodes ()
            .OfType<BlockSyntax>()
            .First();

        return new Node()
        {
            Value = new Value()
            {
                Block = block
            }
        };
    }
    #endregion
}
