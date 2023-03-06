using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using Organizer.Tree;
using Organizer.Client;

namespace Organizer.Controller
{
    public static class Roslyn
    {
        public static IEnumerable<ClassDeclarationSyntax> GetClasses(this IEnumerable<SyntaxTree> trees) 
            => trees.SelectMany(tree => tree.GetClasses());
        private static IEnumerable<ClassDeclarationSyntax> GetClasses(this SyntaxTree tree) 
            => tree
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>();
        public static ClassDeclarationSyntax GetOrganizerClass(this IEnumerable<ClassDeclarationSyntax> classes) 
            => classes.FirstOrDefault(@class => @class.BaseList.Types.Any(t => t.Type.ToString() == nameof(OrganizerServices)));

        public static IEnumerable<BlockSyntax> GetBlockSyntaxes(this ConstructorDeclarationSyntax organizerConstructor)
            => organizerConstructor?
            .DescendantNodes()
            .OfType<BlockSyntax>();


        public static ConstructorDeclarationSyntax GetOrganizerConstructor(this ClassDeclarationSyntax organizerClass) 
            => organizerClass?
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .SingleOrDefault();

        internal static ConstructorDeclarationSyntax GetOrganizerConstructor(this Node root)
            => root
            .Value
            .Block
            .SyntaxTree
            .GetClasses()
            .GetOrganizerClass()
            .GetOrganizerConstructor();

    }
}
