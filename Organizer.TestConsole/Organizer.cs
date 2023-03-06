using Organizer.Client.Attributes;
using Organizer.Client;

namespace Organizer.TestConsole
{
    class Organizer : OrganizerServices
    {
        [From("C:\\Users\\a\\Source\\Repos\\Organizer-GraduationProject\\Organizer.Generator\\Organizer.Tree")]
        [To("C:\\Users\\a\\source\\repos\\Organizer-GraduationProject\\Organizer.TestConsole")]
        public Organizer()
        {
            CreateFolder("GeneratorResultFile");
            {
                UpdateType("Builder", "Heeed");
                IgnoreType("HeaderHandler");
                ContainTypes("H");
                
            }
        }
    }
}
