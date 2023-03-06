using Organizer.Tree;

namespace Organizer.Services.Tests.TypeServicesTests;

//first 4 function in ContainTypeService will tested when run generator 
public class ContainTypeServiceTests
{
    [Fact]
    public void GetTypesToCreateByNames_typesNameToCreateAndTypes_SubsetOfTypes()
    {
        //Arrange
        var typesNameToCreate = new[] { "Clss" };

        var types = CSharpSyntaxTree
            .ParseText("""
            class Clss { }
            enum MyEnum { }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        //Act
        var method = typeof(ContainTypeServcie)
            .GetMethod("GetTypesToCreateByNames",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var typesToCreate = (IEnumerable<BaseTypeDeclarationSyntax>?)method!
            .Invoke(null, new object[] { typesNameToCreate, types })!;

        //Assert
        Assert.Single(typesToCreate);
    }

    [Fact]
    public void GetTypesToCreateByPatterns_typesNameToCreateAndTypes_SubsetOfTypes()
    {
        //Arrange
        var typesNameToCreate = new[] { "Clss" };

        var types = CSharpSyntaxTree
            .ParseText("""
            class Clss { }
            enum MyEnum { }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        //Act
        var method = typeof(ContainTypeServcie)
            .GetMethod("GetTypesToCreateByNames",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var typesToCreate = (IEnumerable<BaseTypeDeclarationSyntax>?)method!
            .Invoke(null, new object[] { typesNameToCreate, types })!;

        //Assert
        Assert.Single(typesToCreate);
    }

    [Fact]
    public void GetPrimaryBlockInvocations_NodeValue_SubsetOfInvocations()
    {
        //Arrange
        var block = CSharpSyntaxTree
        .ParseText("""
            {
                PrimaryBlockInvocation();
                {
                    SecoundBlockInvocation();
                }
                PrimaryBlockInvocation();
                {
                    SecoundBlockInvocation();
                }
                PrimaryBlockInvocation();
            }
            """)
        .GetRoot()
        .DescendantNodes()
        .OfType<BlockSyntax>()
        .First();

        var node = new Node()
        {
            Value = new Value()
            {
                Block = block
            }
        };


        //Act
        var method = typeof(ContainTypeServcie)
            .GetMethod("GetPrimaryBlockInvocations",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var actualInvocations =
            (IEnumerable<InvocationExpressionSyntax>?)method!
            .Invoke(null, new[] { node.Value })!;

        //Assert
        Assert.True(actualInvocations.Count() == 3);
        Assert.All(actualInvocations
            , invoc => invoc.ToString().Contains("Primary"));
    }

    [Fact]
    public void GetFullTargetPath_targetPathAndNode_CompainTargetPathWithPathFromCreateFolderService()
    {
        //Arrange
        var targetPath = "TargetPath";

        var createFolderInvocation = CSharpSyntaxTree
        .ParseText("CreateFolder(\"CreateFolderPath\")")
        .GetRoot()
        .DescendantNodes()
        .OfType<InvocationExpressionSyntax>();

        var node = new Node()
        {
            Value = new Value()
            {
                Header = createFolderInvocation
            }
        };

        var expectedFullTargetPath = Path
            .Combine(targetPath, "CreateFolderPath")
            .Replace("\\\\", "\\")
            .Replace("\\", "\\\\");

        //Act
        var method = typeof(ContainTypeServcie)
            .GetMethod("GetFullTargetPath",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var acutalFullTargetPath = (string?)method!
            .Invoke(null, new object[] { targetPath, node })!;

        //Assert
        Assert.Equal(acutalFullTargetPath, expectedFullTargetPath);
    }
}
    