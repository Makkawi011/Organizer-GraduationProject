using Microsoft.CodeAnalysis;

using Organizer.Controller;

namespace Organizer.Generator
{
    [Generator]
    public class OrganizerGen : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        { }

        public void Execute(GeneratorExecutionContext context)
        {
            context
                .Compilation
                .SyntaxTrees
                .GetClasses()
                .FindOrganizerClass()
                .FindOrganizerConstructor()
                .GetBlockSyntaxes()
                .BuildFileStructureTree()
                .ImplementOrganizerServices();
        }
    }
}