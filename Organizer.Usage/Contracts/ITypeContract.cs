namespace Organizer.Client.Contracts
{
    public interface ITypeContract
    {
        void ContainType(string typeName);

        void ContainTypes(string pattern);

        void ContainTypes(string pattern, string except);

        void IgnoreType(string typeName);

        void IgnoreTypes(string pattern);

        void UpdateType(string oldTypeName, string newTypeName);

        void UpdateTypes(string pattern, string updateName);

        void UpdateTypes(string pattern, string updateName, string except);
    }
}