namespace Organizer.Client.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class From : Attribute
    {
        public From(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }
        }
    }
}