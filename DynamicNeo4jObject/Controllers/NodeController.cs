using DynamicNeo4jObject.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;
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
        public async Task<IActionResult> CreateModel(NodeDto dto)
        {
            var companyName = "SomeCompany";

            var node = new Node
            {
                UniqueName = $"{companyName}_{dto.Name}",
                DisplayName = dto.Name,
                Properties = dto.Properties
            };

             await _client.Cypher
               .Merge($"(n:{node.UniqueName} {{Id: $id}})")
               .OnCreate()
               .Set("n = $nodeData")
               .WithParams(new
               {
                   id = node.Id,
                   nodeData = new
                   {
                       node.Id,
                       UniqueName = node.UniqueName,
                       DisplayName = node.DisplayName
                   }
               })
               .ForEach("(prop in $properties | MERGE (n)-[:HAS_PROPERTY]->(p:Property {Name: prop.Name}) SET p.Value = prop.Value)")
               .WithParam("properties", node.Properties)
               .ExecuteWithoutResultsAsync();

            return Ok("created");

        }

        [HttpGet("GetNodes/{NodeName}")]
        public async Task<IActionResult> FetchNodes(string NodeName)
        {
            var result = await _client.Cypher
            .Match($"(n:{NodeName})")
             .Where((Node n) => n.UniqueName == NodeName)
             .OptionalMatch("(n)-[:HAS_PROPERTY]->(p:Property)")
             .Return((n, p) => new
             {
                 Node = n.As<Node>(),
                 Properties = p.CollectAs<Property>()
             })
             .ResultsAsync;


            if (result != null)
            {
                return Ok(result);
            }
            return null;
        }
    }
}
