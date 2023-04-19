namespace Organizer.Services.Tests;

public class HelpersTests
{
    [Fact]
    public void GetInvocations_WhenTheInputConstructorDeclarationSyntax_ReturnsCollectionOfInvocs()
    {
        // Arrange
        var code = """
            class MyClass
            {
                public MyClass()
                {
                    invoc1();
                    invoc2();
                    invoc3();
                }
            }
            """;
        var ctor = CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .Single();

        var expectedInvocs = ctor
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        //Act
        var actualInvocs = ctor
            .GetInvocations();

        //Assert
        Assert.Equivalent(expectedInvocs, actualInvocs);
    }

    [Fact]
    public void GetTypeName_WhenInputArgumentSyntax_ReturnTheActualParameters()
    {
        // Arrange
        var code = @"invoc(nameof(Type))";

        var argumentSyntax =
            CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<ArgumentSyntax>()
            .First();

        // Act
        var actual = argumentSyntax.GetTypeName();

        // Assert
        Assert.Equal("Type", actual);
    }

    [Fact]
    public void ConvertToBaseTypeDeclarationSyntax_WhenTheInputString_ReturnsBaseTypeDeclarationSyntax()
    {
        // Arrange
        var code = """
            class MyClass
            {
                public MyClass()
                {
                    invoc1();
                    invoc2();
                    invoc3();
                }
            }
            """;
        var expectedType = CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .Single();

        //Act
        var actualType = Helpers.ConvertToBaseTypeDeclarationSyntax(code);

        //Assert
        Assert.Equivalent(expectedType.ToString(), actualType.ToString());
    }

    [Fact]
    public void GetInvocationsByName_CollectionOfInvocation_SubsetInvocationBySpicifecName()
    {
        var code = """
            OrganizerService();
            invoc1();
            OrganizerService();
            invoc2();
            OrganizerService();
            invoc3();
            """;
        var invocations = CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        var expectedInvocations = invocations
            .Where(invoc => invoc.ToString().Contains("OrganizerService"));

        var actualInvocations = invocations
            .GetInvocationsByName("OrganizerService");

        Assert.Equivalent(expectedInvocations, actualInvocations);
    }

    [Fact]
    public void GetSingleParamsOf_CollectionOfInvocation_ParametersOfSpicificInvocation()
    {
        var code = """
            OrganizerService("type1");
            invoc1();
            OrganizerService(nameof(type2));
            invoc2(555);
            """;
        var invocations = CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        var expectedParams = new[] { "type1", "type2" };

        var actualParams = invocations
            .GetSingleParamsOf("OrganizerService");

        Assert.Equivalent(expectedParams, actualParams);
    }

    [Fact]
    public void GetMultParamsOf_CollectionOfInvocation_ParametersOfSpicificInvocation()
    {
        var code = """
            invoc1();
            OrganizerService("type1",nameof(type2));
            invoc2(555);
            """;
        var invocations = CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        var expectedParams = new[] { "type1", "type2" };

        var actualParams = invocations
            .GetMultParamsOf("OrganizerService")
            .First();

        Assert.Equivalent(expectedParams, actualParams);
    }
}