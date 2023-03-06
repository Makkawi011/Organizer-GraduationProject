using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using Organizer.Client;
using Organizer.Tree;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Organizer.Generator.Services.FolderServices
{
    public static class CreateFolderService
    {
        public static void CreateForFolders
            (this IEnumerable<Node> leafs,
            string targetPath)
        {
            targetPath = targetPath
                .Replace("\\\\", "\\")
                .Replace("\\", "\\\\");

            foreach (var node in leafs)
            {
                var path = node
                    .Value?.Header
                    .LastOrDefault(invoc => invoc.IsName(nameof(OrganizerServices.CreateFolder)))
                    .ArgumentList.Arguments
                    .SingleOrDefault()
                    .GetParameterValue();


                var fullPath = Path.Combine(targetPath, path)
                    .Replace("\\\\", "\\")
                    .Replace("\\", "\\\\");
                

                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
            }
        }
    }
}
