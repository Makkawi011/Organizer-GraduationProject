using System;
using System.Collections.Generic;
using System.Linq;



using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Organizer.Helpers
{
    internal static class Attribute
    {
        internal static IEnumerable<AttributeSyntax>? GetAttributes(this ConstructorDeclarationSyntax? organizerCtor, Type attribute)
            => organizerCtor!
            .AttributeLists!
            .SelectMany(a => a.Attributes)
            .Where(a => a.Name.ToString() == attribute.Name);
    }
}
