using Organizer.Client;
using Organizer.Client.Attributes;

namespace Organizer.TestConsole;

internal class Organizer : OrganizerServices
{
    [From("C:\\Users\\a\\Desktop\\UnStructuredCode.cs")]
    [To("C:\\Users\\a\\Source\\Repos\\Organizer\\Organizer.TestConsole")]
    public Organizer()
    {
        CreateFolder("OrganizedCode");
        {
            CreateFolder("Requests");
            {
                ContainTypes("Request");
                IgnoreType("Garbage");
            }
            CreateFolder("Responses");
            {
                UpdateTypes("Response", "Res");
                ContainTypes("Res");
            }
            CreateFolder("DataModels");
            {
                ContainTypes("Model");
            }
        }
    }
}