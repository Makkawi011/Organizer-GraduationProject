using Organizer.Generator.Helpers;
using Organizer.Generator.Services.TypeServices;
using Organizer.Tree;

namespace Organizer.Generator
{
    public static class Organizer
    {
        public static void ImplementOrganizerServices(this Node? root)
        {
            var orgCtor = root
                .GetOrganizerConstructor()!;

            var toDirPath = orgCtor!
                .GetTargetDirectoryPath();

            orgCtor
                .GetCustomerTypeDeclarationSyntaxes()!
                .IgnoreForTypes(orgCtor);

            //    .UpdateTypesAndTypeImp()
            //    .CreateFilesImp(toDirPath)
            //    .CreateTypesAndTypeImp();

            // Add the source code to the compilation
            //context.AddSource($"{organizerClass.Identifier.Text}.g.cs", organizerClass.Identifier.Text);

        }

    }
}
