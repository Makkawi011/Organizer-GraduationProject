using Organizer.Helpers;
using Organizer.Tree;

namespace Organizer
{
    internal static class Solver
    {
        public static void ImplementOrganizerServices(this Node? root)
        {
            var orgCtor = root
                .GetOrganizerConstructor();

            var toDirPath = orgCtor!
                .GetTargetDirectoryPath();
            var customerTypeSyntaxes = orgCtor
                .GetCustomerTypeDeclarationSyntaxes();



            // Add the source code to the compilation
            //context.AddSource($"{organizerClass.Identifier.Text}.g.cs", organizerClass.Identifier.Text);

        }

    }
}
