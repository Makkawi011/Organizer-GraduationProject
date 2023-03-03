using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using Organizer.Generator.Services.FolderServices;
using Organizer.Generator.Services.TypeServices;
using Organizer.Tree;

namespace Organizer.Controller
{
    public static class Servicer
    {
        public static void ImplementOrganizerServices(this List<Node>? nodes ,GeneratorExecutionContext context)
        {


            nodes
                .Where(node => node.IsLeaf)
                .CreateForFolders(toDirPath);

                .IgnoreForTypes(orgCtor)
                .UpdateForTypes(orgCtor)

        }

    }
}
