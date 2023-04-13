namespace Organizer.Services.Tests.TypeServicesTests;

public class UpdateTypeServiceTests
{
    [Fact]
    public void UpdaterByTypeName_TypeAndUpdateName_UpdatedType()
    {
        // Arrange
        var oldType = "class Class1 { Class2 class2 = new(); }";
        var updateParameters = new[]
        {
            new [] { "Class2" , "NewClass" }
        };

        var expectedType = "class Class1 { NewClass class2 = new(); }";
        
        // Act
        var method = typeof(UpdateTypeService)
            .GetMethod("UpdaterByTypeName",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var actualType = (string?)method!
            .Invoke(null, new object[] { oldType, updateParameters })!;
        
        // Assert
        Assert.Equal(expectedType, actualType);

    }

    [Fact]
    public void ToUpdatedTypesNames_TypeAndUpdatePattern_UpdatedTypeNames()
    {
        // Arrange
        var oldTypes = CSharpSyntaxTree
            .ParseText("""
            class OldClass { }
            class Class1 { OldClass class2 = new(); }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        var updateParametersPatterns = new[]
        {
            new [] { "Old", "New" }
        };

        var updateParametersNamesExpected = new[]
        {
            new [] { "OldClass", "NewClass" }
        };

        // Act
        var method = typeof(UpdateTypeService)
            .GetMethod("ToUpdatedTypesNames",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var updateParametersNamesActual = (IEnumerable<IEnumerable<string>>?)method!
            .Invoke(null, new object[] { updateParametersPatterns, oldTypes })!;

        // Assert
        Assert.Equivalent(updateParametersNamesExpected, updateParametersNamesActual);

    }

    [Fact]
    public void UpdateForTypesByName_TypesAndInvocations_UpdatedType()
    {
        // Arrange
        var oldTypes = CSharpSyntaxTree
            .ParseText("""
            class OldClass { }
            class Class1 { OldClass class2 = new(); }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        var newTypesExpected = CSharpSyntaxTree
            .ParseText("""
            class NewClass { }
            class Class1 { NewClass class2 = new(); }
            """)
            .GetRoot()
            .NormalizeWhitespace()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        var invocations = CSharpSyntaxTree
            .ParseText("""
            invoc1();
            UpdateType("OldClass","NewClass" );
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        // Act
        var method = typeof(UpdateTypeService)
            .GetMethod("UpdateForTypesByName",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var newTypesActual = (IEnumerable<BaseTypeDeclarationSyntax>?)method!
            .Invoke(null, new object[] {oldTypes , invocations })!;

        // Assert
        Assert.Equivalent(
            newTypesExpected.Select(t => t.ToString()),
            newTypesActual.Select(t => t.ToString())
            );

    }

    [Fact]
    public void UpdateForTypesByPattern_TypesAndInvocations_UpdatedType()
    {
        // Arrange
        var oldTypes = CSharpSyntaxTree
            .ParseText("""
            class OldClass { }
            class Class1 { OldClass class2 = new(); }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        var newTypesExpected = CSharpSyntaxTree
            .ParseText("""
            class NewClass { }
            class Class1 { NewClass class2 = new(); }
            """)
            .GetRoot()
            .NormalizeWhitespace()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        var invocations = CSharpSyntaxTree
            .ParseText("""
            invoc1();
            UpdateTypes("Old","New" );
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        // Act
        var method = typeof(UpdateTypeService)
            .GetMethod("UpdateForTypesByPattern",
            BindingFlags.NonPublic |
            BindingFlags.Static);

        var newTypesActual = (IEnumerable<BaseTypeDeclarationSyntax>?)method!
            .Invoke(null, new object[] { oldTypes, invocations })!;

        // Assert
        Assert.Equivalent(
            newTypesExpected.Select(t => t.ToString()),
            newTypesActual.Select(t => t.ToString())
            );

    }

    [Fact]
    public void UpdateForTypes_TypesAndCtor_UpdatedType()
    {
        // Arrange
        var oldTypes = CSharpSyntaxTree
            .ParseText("""
            class OldClass { }
            class Class1 { OldClass class2 = new(); }
            """)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        var newTypesExpected = CSharpSyntaxTree
            .ParseText("""
            class NewClass { }
            class NewClass1 { NewClass class2 = new(); }
            """)
            .GetRoot()
            .NormalizeWhitespace()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        var ctor = CSharpSyntaxTree.ParseText("""
            class Clss
            {
                public Clss()
                {
                    invoc1();
                    UpdateTypes("Old", "New");
                    UpdateType(nameof(Class1), "NewClass1");
                }
            }
            """).GetRoot()
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .First();

        // Act
        var newTypesActual = oldTypes.UpdateForTypes(ctor);

        // Assert
        Assert.Equivalent(
            newTypesExpected.Select(t => t.ToString()),
            newTypesActual.Select(t => t.ToString())
            );
    }
}
