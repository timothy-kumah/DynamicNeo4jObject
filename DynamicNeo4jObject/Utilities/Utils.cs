using DynamicNeo4jObject.Models;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json.Serialization;

namespace DynamicNeo4jObject.Utilities
{
    public static class Utils
    {
        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }

        public static ICypherFluentQuery AssignRelationships(this ICypherFluentQuery query, List<RelationshipObj> rels)
        {

            return query.With("n")
            .OptionalMatch($"(relatedNode:{rels[0].TargetObjectLabel} {{Id: '{rels[0].TargetObjectId}'}})")
            .Merge($"(n)-[:{rels[0].Label}{{From:'{rels[0].SourceObjectId}'," +
                $"To:'{rels[0].TargetObjectId}'}}]->(relatedNode)");
        }   
    }
}
