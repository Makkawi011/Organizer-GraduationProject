using System.Diagnostics;

namespace Organizer.Controller.Tests;

public class FilesTests
{
    [Fact]
    public void GetBaseTypeSyntaxes_ReturnsBaseTypeSyntaxCollection_WhenTheInputPieceCode()
    {
        // Arrange

        var types = @"
        class Type1 { }
        struct Type2 { }
        enum Type3  { }";

        var expectedTypes = CSharpSyntaxTree.ParseText(types)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        // Act

        var method = typeof(Files)
            .GetMethod("GetBaseTypeSyntaxes",
            BindingFlags.NonPublic |
            BindingFlags.Static,
            new Type[] { typeof(string) });

        var actualTypes = (IEnumerable<BaseTypeDeclarationSyntax>?)method!
            .Invoke(null, new[] { types });

        // Assert

        Assert.Equivalent(actualTypes!.Select(type => type.ToString()),
            expectedTypes.Select(type => type.ToString()));
    }

    [Fact]
    public void GetBaseTypeSyntaxes_ReturnsBaseTypeSyntaxCollection_WhenTheInputPieceCodeCollection()
    {
        // Arrange

        var type1 = "class Type1 { }";
        var type2 = "struct Type2 { }";
        var type3 = "enum Type3  { }";
        var types = type1 + type2 + type3;
        var typesCollection = new[] { type1, type2, type3 };

        var expectedTypes = CSharpSyntaxTree.ParseText(types)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        // Act

        var method = typeof(Files)
            .GetMethod("GetBaseTypeSyntaxes",
            BindingFlags.NonPublic |
            BindingFlags.Static,
            new Type[] { typeof(IEnumerable<string>) });

        var actualTypes = (IEnumerable<BaseTypeDeclarationSyntax>?)method!
            .Invoke(null, new[] { typesCollection });

        // Assert

        Assert.Equivalent(actualTypes!.Select(type => type.ToString()), expectedTypes.Select(type => type.ToString()));
    }

    [Fact]
    public void GetFilesContents_ReturnsStringsCollection_TheInputCurrentFilePath()
    {
        //Arrange
        string path = new StackTrace(true)
            .GetFrame(0)!
            .GetFileName()!;

        var paths = new[] { path };

        var expectedContent = File.ReadAllText(path);

        //Act
        var method = typeof(Files)
            .GetMethod("GetFilesContents",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var actualContent = (IEnumerable<string>?)method!
            .Invoke(null, new[] { paths });

        //Assert 
        Assert.Equal(expectedContent , actualContent!.Single());

    }

    [Fact]
    public void GetCSFilesPaths_ReturnsPathsCollection_TheInputCurrentDirectoryPath()
    {
        //Arrange
        string currentFilePath = new StackTrace(true)
            .GetFrame(0)!
            .GetFileName()!;

        string path = Path.GetDirectoryName(currentFilePath)!;


        //Act
        var method = typeof(Files)
            .GetMethod("GetCSFilesPaths",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var actualPaths = (IEnumerable<string>?)method!
            .Invoke(null, new[] { path })!;

        //Assert 
        Assert.Contains(actualPaths, p => p == currentFilePath);

    }
    [Fact]
    public void GetPath_ReturnsPath_TheInputAttributeHasCurrentFilePath()
    {
        //Arrange
        string currentFilePath = new StackTrace(true)
            .GetFrame(0)!
            .GetFileName()!;

        var attributeSyntax = CSharpSyntaxTree
            .ParseText($"[From(\"{currentFilePath}\")]")
            .GetRoot()
            .DescendantNodes()
            .OfType<AttributeSyntax>()
            .Single();

        //Act
        var method = typeof(Files)
            .GetMethod("GetPath",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var actualPath = (string) method!
            .Invoke(null, new[] { attributeSyntax })!;

        //Assert 
        Assert.Equal(actualPath , currentFilePath);

    }
    [Fact]
    public void GetPaths_ReturnsStringPathsCollection_TheInputAttributesHasCurrentDirectoryPath()
    {
        //Arrange
        string currentFilePath = new StackTrace(true)
            .GetFrame(0)!
            .GetFileName()!;

        string currentDirectory = Path.GetDirectoryName(currentFilePath)!;

        var attributeSyntax = CSharpSyntaxTree
            .ParseText($"[From(\"{currentDirectory}\")]")
            .GetRoot()
            .DescendantNodes()
            .OfType<AttributeSyntax>();

        //Act
        var method = typeof(Files)
            .GetMethod("GetPaths",
            BindingFlags.NonPublic |
            BindingFlags.Static)!;

        var actualPaths = (IEnumerable<string>?)method
            .Invoke(null, new[] { attributeSyntax })!;

        //Assert 
        Assert.Contains(actualPaths, p => p == currentFilePath);

    }

    [Fact]
    public void GetCustomerTypeDeclarationSyntaxes_ReturnsBaseTypesCollection_TheInputAttributesHasCurrentDirectoryPath()
    {
        //Arrange
        string currentFilePath = new StackTrace(true)
            .GetFrame(0)!
            .GetFileName()!;

        string currentDirectory = "\""+ Path.GetDirectoryName(currentFilePath) +"\"";

        
        var organizerCtor = CSharpSyntaxTree.ParseText(@"
        class Organizer : OrganizerServices
        {
            [From("+$"{currentDirectory})]" +
            "public Organizer()" +
            "{" +
            "}" +
         "}")
        .GetRoot()
        .DescendantNodes()
        .OfType<ConstructorDeclarationSyntax>()
        .First();

        //Act

        var actualtypes = organizerCtor
            .GetCustomerTypeDeclarationSyntaxes()!;

        //Assert 
        Assert.Contains(actualtypes
            .Select(t => t.Identifier.Text)
            , name => name == nameof(FilesTests));

    }
    [Fact]
    public void GetTargetDirectoryPath_Returnsstring_TheInputAttributesHasCurrentDirectoryPath()
    {
        //Arrange
        string currentFilePath = new StackTrace(true)
            .GetFrame(0)!
            .GetFileName()!;

        string currentDirectory = Path.GetDirectoryName(currentFilePath)!;


        var organizerCtor = CSharpSyntaxTree.ParseText(@"
        class Organizer : OrganizerServices
        {
            [To(" + "\""+ $"{currentDirectory}"+"\")]"+
            "public Organizer()" +
            "{" +
            "}" +
         "}")
        .GetRoot()
        .DescendantNodes()
        .OfType<ConstructorDeclarationSyntax>()
        .First();

        //Act
        var actualpath = organizerCtor.GetTargetDirectoryPath();

        //Assert 
        Assert.Equal(actualpath,currentDirectory);

    }

}
