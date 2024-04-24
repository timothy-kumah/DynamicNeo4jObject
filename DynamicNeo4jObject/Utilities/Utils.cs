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
            var cypherQuery = query.With("n");
            var i = 0;
            foreach (var rel in rels)
            {
                cypherQuery = cypherQuery
                    .OptionalMatch($"(relatedNode{i}:{rel.TargetObjectLabel} {{Id: '{rel.TargetObjectId}'}})")
                    .With($"n,relatedNode{i}")
                    .ForEach($"(ignoreMe IN CASE WHEN relatedNode{i} IS NOT NULL THEN [1] ELSE [] END | " +
                         $"CREATE (n)-[:{rel.Label}{{From:'{rel.SourceObjectId}',To:'{rel.TargetObjectId}'}}]->(relatedNode{i}))")
                    .With("n");

                    //.Create($"(n)-[:{rel.Label}{{From:'{rel.SourceObjectId}',To:'{rel.TargetObjectId}'}}]->(relatedNode{i})");
                i++;
            }

            return cypherQuery;
        }   
    }
}
