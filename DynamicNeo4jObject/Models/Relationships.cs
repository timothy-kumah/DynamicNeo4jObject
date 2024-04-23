namespace DynamicNeo4jObject.Models
{
    public class RelationshipObj
    {
        public string Label { get; set; }

        public string TargetObjectLabel { get; set; }

        public string TargetObjectId { get; set; }

        public string SourceObjectLabel { get; set; }

        public string SourceObjectId { get; set; }
    }
}
