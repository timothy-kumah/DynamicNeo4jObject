using DynamicNeo4jObject.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Node = DynamicNeo4jObject.Models.Node;

namespace DynamicNeo4jObject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeController : Controller
    {
        private IGraphClient _client;
        public NodeController(IGraphClient client)
        {
            _client = client;
        }

        [HttpPost("CreateNode")]
        public async Task<IActionResult> CreateModel(Dictionary<string,string> dto)
        {
            //var companyName = "SomeCompany";

            //var node = new Node
            //{
            //    UniqueName = $"{companyName}_{dto.Name}",
            //    DisplayName = dto.Name,
            //    Properties = dto.Properties
            //};

            //await _client.Cypher
            //.Merge($"(n:{node.UniqueName} {{Id: $id}})")
            //.OnCreate()
            //.Set("n = $nodeData")
            //.WithParams(new
            //{
            //    id = node.Id,
            //    nodeData = new
            //    {
            //    node.Id,
            //        UniqueName = node.UniqueName,
            //        DisplayName = node.DisplayName
            //    }
            //})
            ////.ForEach("(prop in $properties | MERGE (n)-[:HAS_PROPERTY]->(p:Property {Name: prop.Name}) SET p.Value = prop.Value)")
            //.ForEach("(prop in $properties | n.")
            //.WithParam("properties", node.Properties)
            //.ExecuteWithoutResultsAsync();
            //var dynamicProperties = new Dictionary<string, object>
            //{
            //    { "name", "John" },
            //    { "age", 30 },
            //    { "city", "New York" }
            //    // Add more properties dynamically if needed
            //};

            //var companyName = "SomeCompany";

            //var node = new Node
            //{
            //    UniqueName = $"{companyName}_{dto.Name}",
            //    DisplayName = dto.Name,               
            //};

            // Create a node with dynamic properties using a Cypher query
            var companyName = "SomeCompany";
            var unique_name = $"{companyName}_{dto["Name"]}";
            var guid = Guid.NewGuid();
            dto.Add("Id",guid.ToString());
            dto.Add("UniqueName", unique_name);
            await _client.Cypher
                .Create($"(n:{unique_name})")
                .Set("n = $dynamicProperties")
                .WithParams(new {
                    dynamicProperties= dto
                })
                .ExecuteWithoutResultsAsync();

            return Ok("created");

        }

        [HttpGet("GetNodes/{NodeName}")]
        public async Task<IActionResult> FetchNodes(string NodeName)
        {
            var result = await _client.Cypher
            .Match($"(n:{NodeName})")
             .Return( n => n.As<string>())
             .ResultsAsync;

            if (result.Any())
            {
                var items = result.Select(x => JObject.Parse(x)["data"]);

                var serialized = JsonConvert.SerializeObject(items, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new LowercaseContractResolver()
                });

                return Ok(serialized);
            }
            return null;
        }

        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
    }
}
