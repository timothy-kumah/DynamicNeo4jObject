namespace DynamicNeo4jObject.Models
{
    public class CreateNodeRequest
    {
        public Dictionary<string, string> Properties {  get; set; }
        public List<RelationshipObj> Relationships { get; set; }
    }
}
