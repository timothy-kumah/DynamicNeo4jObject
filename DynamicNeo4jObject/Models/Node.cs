namespace DynamicNeo4jObject.Models
{
    public class Node
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UniqueName { get; set; }

        public string DisplayName { get; set; }

        public List<Property> Properties { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
