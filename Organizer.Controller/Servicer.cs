using System.Collections.Generic;
using System.Linq;

using Organizer.Generator.Services.FolderServices;
using Organizer.Generator.Services.TypeServices;
using Organizer.Tree;

namespace Organizer.Controller
{
    public static class Servicer
    {
        public static void ImplementOrganizerServices(this List<Node>? nodes)
        {
            var root = nodes?.GetRoot();
            if (root is null) return;

            var orgCtor = root.GetOrganizerConstructor();
            if (orgCtor is null) return;

            var toDirPath = orgCtor.GetTargetDirectoryPath()!;


            nodes
                .Where(node => node.IsLeaf)
                .CreateForFolders(toDirPath);

            orgCtor.GetCustomerTypeDeclarationSyntaxes()?
                .IgnoreForTypes(orgCtor)
                .UpdateForTypes(orgCtor)
                .CreateForTypes(nodes, toDirPath);

        }

    }
}
