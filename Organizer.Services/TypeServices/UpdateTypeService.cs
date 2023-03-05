using Microsoft.CodeAnalysis.CSharp.Syntax;

using Organizer.Client;
using Organizer.Services;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Organizer.Generator.Services.TypeServices
{
    public static class UpdateTypeService
    {
        public static IEnumerable<BaseTypeDeclarationSyntax> UpdateForTypes
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            ConstructorDeclarationSyntax organizerCtor)
        {
            var invocations = organizerCtor
                .GetInvocations()!;

            return types
                .UpdateForTypesByName(invocations)
                .UpdateForTypesByPattern(invocations);
        }

        private static IEnumerable<BaseTypeDeclarationSyntax> UpdateForTypesByName
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            IEnumerable<InvocationExpressionSyntax> invocations)
        {
            //get updated oldTypes names to get all the oldTypes
            //update name of oldTypes we want to update

            var updatedTypesByName = invocations?
                .GetMultParamsOf(nameof(OrganizerServices.UpdateType));

            var updatedTypes = types
                .Select(type => type.ToString())
                .Select(type => UpdaterByTypeName(type, updatedTypesByName))
                .Select(type => Organizer.Services.Helpers.ConvertToBaseTypeDeclarationSyntax(type));

            return updatedTypes;
        }
        private static IEnumerable<BaseTypeDeclarationSyntax> UpdateForTypesByPattern
            (this IEnumerable<BaseTypeDeclarationSyntax> types,
            IEnumerable<InvocationExpressionSyntax> invocations)
        {
            var updatedTypesByName = invocations
                .GetMultParamsOf(nameof(OrganizerServices.UpdateTypes)) //updatedTypesByPattern
                .ToUpdatedTypesNames(types);

            var updatedTypes = types
                .Select(type => type.ToString())
                .Select(type => UpdaterByTypeName(type, updatedTypesByName))
                .Select(type => Organizer.Services.Helpers.ConvertToBaseTypeDeclarationSyntax(type));


            return updatedTypes;
        }

        private static IEnumerable<IEnumerable<string>> ToUpdatedTypesNames
            (this IEnumerable<IEnumerable<string>> updatedTypesByPattern,
            IEnumerable<BaseTypeDeclarationSyntax> oldTypes) 

            => from patterns in updatedTypesByPattern
               let oldPattern = patterns.First()
               let newPattern = patterns.Last()

               from oldTypeName in oldTypes.Select(type => type.Identifier.Text)
               let newTypeName = Regex.Replace(oldTypeName, oldPattern, newPattern)

               select new List<string>
               {
                   oldTypeName,
                   newTypeName,
               };


        private static string UpdaterByTypeName(string type , IEnumerable<IEnumerable<string>>? parameters)
        {
            foreach (var @params in parameters!)
            {
                var oldTypeName = @params.First();
                var newTypeName = @params.Last();
                type = Regex.Replace(type, oldTypeName, newTypeName);
            }
            return type;
        }

    }

}
