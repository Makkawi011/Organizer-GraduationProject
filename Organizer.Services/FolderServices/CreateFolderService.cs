using Organizer.Client;
using Organizer.Tree;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Organizer.Generator.Services.FolderServices
{
    public static class CreateFolderService
    {
        public static void CreateForFolders
            (this IEnumerable<Node> leafs,
            string targetPath)
        {

            foreach (var node in leafs)
            {
                var path = node
                    .Value?.Header
                    .LastOrDefault(invoc => invoc.IsName(nameof(OrganizerServices.CreateFolder)))
                    .ArgumentList.Arguments
                    .SingleOrDefault()
                    .GetParameterValue();

                var fullPath = Path.Combine(targetPath, path);

                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(targetPath);
            }
        }
    }
}
