using Organizer.Generator.Services.TypeServices;
using Organizer.Tree;

namespace Organizer.Services.Tests.TypeServicesTests;

public class IgnoreTypeServiceTests
{
    [Fact]
    public void IgnoreTypesByPattern_CollectionOfTypesAndInvocations_FillteredTypes()
    {
        // Arrange
        var invocations =
            CSharpSyntaxTree.ParseText("""
            invoc1();
            IgnoreTypes("Garbage");
            invoc2();
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        var types =
            CSharpSyntaxTree.ParseText("""
            class ClssGarbage { }
            struct StructGarbage { }
            enum EnumGarbage { }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        // Act
        var method = typeof(IgnoreTypeService)
            .GetMethod("IgnoreTypesByPattern",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var typesRested = (IEnumerable<BaseTypeDeclarationSyntax>?)method!
            .Invoke(null, new object [] { types , invocations });

        Assert.Empty(typesRested!);
    }

    [Fact]
    public void IgnoreTypesByName_CollectionOfTypesAndInvocations_FillteredTypes()
    {
        // Arrange
        var invocations =
            CSharpSyntaxTree.ParseText("""
            invoc1();
            IgnoreType(nameof(Garbage));
            invoc2();
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        var types =
            CSharpSyntaxTree.ParseText("""
            class Clss { }
            struct Garbage { }
            enum EnumGarbage { }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        // Act
        var method = typeof(IgnoreTypeService)
            .GetMethod("IgnoreTypesByName",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var typesRested = (IEnumerable<BaseTypeDeclarationSyntax>?)method!
            .Invoke(null, new object[] { types, invocations })!;

        Assert.NotEmpty(typesRested);
        Assert.Equal(2, typesRested.Count());
        Assert.Contains(typesRested, t => t.Identifier.Text != "Garbage");
    }

    [Fact]
    public void IgnoreForTypes_CollectionOfTypesAndInvocations_FillteredTypes()
    {
        // Arrange
        var ctor =
            CSharpSyntaxTree.ParseText("""
            class MyClass
            {
                public MyClass()
                {
                    invoc1();
                    IgnoreTypes("Garbage");
                    IgnoreType(nameof(Clss));
                    invoc2();
                }
            }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .Single();

        var types =
            CSharpSyntaxTree.ParseText("""
            class Clss { }
            struct Garbage { }
            enum EnumGarbage { }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        // Act
        var typesRested = types.IgnoreForTypes(ctor);

        // Assert
        Assert.Empty(typesRested);
    }
}
