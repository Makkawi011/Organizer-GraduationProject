
namespace Organizer.Client
{
    public class OrganizerServices : IClientContracts
    {
        public void CreateFolder(string directoryName) { }

        public void ContainType(string typeName) { }
        public void ContainTypes(string pattern) { }
        public void ContainTypes(string pattern, string except) { }
        public void IgnoreType(string typeName) { }
        public void IgnoreTypes(string pattern) { }
        public void UpdateType(string oldTypeName, string newTypeName) { }
        public void UpdateTypes(string pattern, string updateName) { }
        public void UpdateTypes(string pattern, string updateName, string except) { }
    }
}
