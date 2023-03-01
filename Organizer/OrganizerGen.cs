using Microsoft.CodeAnalysis;

using Organizer.Helpers;

namespace Organizer
{
    [Generator]
#pragma warning disable RS1036 // Specify analyzer banned API enforcement setting
    public class OrganizerGen : ISourceGenerator
#pragma warning restore RS1036 // Specify analyzer banned API enforcement setting
    {
        public void Execute(GeneratorExecutionContext context)
        {
            context
            .Compilation
            .SyntaxTrees
            .GetClasses()
            .GetOrganizerClass()
            .GetOrganizerConstructor()
            .GetBlockSyntaxes()
            .BuildFileStructureTree()
            .GetRoot()
            .ImplementOrganizerServices();
        }
        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}

