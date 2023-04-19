namespace Organizer.Controller.Tests;

public class AttributeTests
{
    [Theory]
    [InlineData(typeof(From), nameof(From), 3)]
    [InlineData(typeof(To), nameof(To), 1)]
    public void GetAttributes_ReturnsAttributeSyntaxCollection(Type attType, string attName, int attNumber)
    {
        // Arrange

        var organizerCtor = CSharpSyntaxTree.ParseText(@"
        class Organizer : OrganizerServices
        {
            [From(""\\FromPath1"")]
            [From(""\\FromPath2"")]
            [From(""\\FromPath3"")]
            [To(""\\ToPath"")]
            public Organizer()
            {
            }
        }")
            .GetRoot()
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .First();

        // Act
        var fromAttributes = organizerCtor.GetAttributes(attType);

        // Assert
        Assert.True(fromAttributes.Count() == attNumber);
        Assert.True(fromAttributes.All(att => att.Name.ToString().Equals(attName)));
    }
}