using DynamicNeo4jObject.Models;
using DynamicNeo4jObject.Utilities;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static DynamicNeo4jObject.Utilities.Utils;


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
        public async Task<IActionResult> CreateModel(CreateNodeRequest dto)
        {

            var companyName = "SomeCompany";
            var unique_name = $"{companyName}_{dto.Properties["Name"]}";
            var guid = Guid.NewGuid().ToString();
            dto.Properties.Add("Id",guid);
            dto.Properties.Add("UniqueName", unique_name);

            dto.Relationships.ForEach(x =>
            {
                x.SourceObjectLabel = unique_name;
                x.SourceObjectId = guid;
            });
            await _client.Cypher
                .Create($"(n:{unique_name})")               
                .AssignRelationships(dto.Relationships)
                .Set("n = $dynamicProperties")
                .WithParams(new {
                    dynamicProperties= dto.Properties,
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
            .Where("n.Id = $Id")
            .WithParam("Id",Id)
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
            return Ok();
        }
    }
}
