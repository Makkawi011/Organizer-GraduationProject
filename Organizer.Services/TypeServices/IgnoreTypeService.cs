using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using Organizer.Client;
using Organizer.Services;
using Organizer.Tree;

namespace Organizer.Generator.Services.TypeServices
{
    public static class IgnoreTypeService
    {
        public static IEnumerable<BaseTypeDeclarationSyntax> IgnoreForTypes
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            ConstructorDeclarationSyntax organizerCtor)
        {
            var invocations = organizerCtor
                .GetInvocations();

            return types
                .IgnoreTypesByName(invocations)
                .IgnoreTypesByPattern(invocations);
        }

        private static IEnumerable<BaseTypeDeclarationSyntax> IgnoreTypesByName
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            IEnumerable<InvocationExpressionSyntax>? invocations)
        {
            //get ignored types names to get all the types
            //without those have exactly same name

            var ignoredTypesByName = invocations
                .GetSingleParamsOf(nameof(OrganizerServices.IgnoreType));

            return types
                .Where(baseType => !ignoredTypesByName
                    .Any(ignoredTypeName => ignoredTypeName
                        .Equals(baseType.Identifier.Text))); 
        }

        private static IEnumerable<BaseTypeDeclarationSyntax> IgnoreTypesByPattern
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            IEnumerable<InvocationExpressionSyntax> invocations)
        {
            //get ignored patterns to get all the types
            //without those have ignored patterns in it's name

            var ignoredTypesByPatterns = invocations
                .GetSingleParamsOf(nameof(OrganizerServices.IgnoreTypes));

            return types
                .Where(baseType => !ignoredTypesByPatterns
                    .Any(ignoredTypePattern => ignoredTypePattern
                        .Contains(baseType.Identifier.Text)));

        }


    }
}
