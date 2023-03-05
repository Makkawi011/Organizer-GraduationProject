using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using Organizer.Client;
using Organizer.Services;

namespace Organizer.Generator.Services.TypeServices
{
    public static class IgnoreTypeService
    {
        public static IEnumerable<BaseTypeDeclarationSyntax> IgnoreForTypes
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            ConstructorDeclarationSyntax organizerCtor)
        {
            var invocations = organizerCtor
                .GetInvocations()!;

            return types
                .IgnoreTypesByName(invocations)
                .IgnoreTypesByPattern(invocations);
        }

        private static IEnumerable<BaseTypeDeclarationSyntax> IgnoreTypesByName
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            IEnumerable<InvocationExpressionSyntax> invocations) =>
            //get ignored types names to get all the types
            //without those have exactly same name
            from type in types
            let typeName = type.Identifier.Text.ToString()
            let ignoredNames = invocations.GetSingleParamsOf(nameof(OrganizerServices.IgnoreType))
            where ! ignoredNames.Any(ignrName => typeName.Equals(ignrName))
            select type;


        private static IEnumerable<BaseTypeDeclarationSyntax> IgnoreTypesByPattern
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            IEnumerable<InvocationExpressionSyntax> invocations) =>
            //get ignored patterns to get all the types
            //without those have ignored patterns in it's name
            from type in types
            let typeName = type.Identifier.Text.ToString()
            let ignoredPatterns = invocations.GetSingleParamsOf(nameof(OrganizerServices.IgnoreTypes))
            where ! ignoredPatterns.Any(ignrPtrn => typeName.Contains(ignrPtrn))
            select type;


    }
}
