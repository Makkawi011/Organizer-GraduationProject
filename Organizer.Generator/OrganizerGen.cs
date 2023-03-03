using Microsoft.CodeAnalysis;

using Organizer.Controller;

namespace Organizer.Generator
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
            .GetClasses()?
            .GetOrganizerClass()
            .GetOrganizerConstructor()
            .GetBlockSyntaxes()
            .BuildFileStructureTree()
            .ImplementOrganizerServices(context);
        }
        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}

