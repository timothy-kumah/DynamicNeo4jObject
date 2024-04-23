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

        [HttpGet("GetNodes/{NodeName}/{id}")]
        public async Task<IActionResult> FetchNodes(string NodeName,string Id)
        {
            var result = await _client.Cypher
            .Match($"(n:{NodeName})")
             .Return(n => n.As<string>())
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
