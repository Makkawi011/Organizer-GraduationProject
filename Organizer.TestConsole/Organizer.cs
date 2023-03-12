using Organizer.Client;
using Organizer.Client.Attributes;

namespace Organizer.TestConsole;

class Organizer : OrganizerServices
{
    [From("C:\\Users\\a\\Source\\Repos\\Organizer-GraduationProject\\Organizer.Generator\\Organizer.Tree")]
    [To("C:\\Users\\a\\source\\repos\\Organizer-GraduationProject\\Organizer.TestConsole")]
    public Organizer()
    {
        CreateFolder("Path1");
        {
            CreateFolder("Path2");
            {
                CreateFolder("Path3");
                {
                    //ContainTypes("H");
                }
            }
        }
    }
}