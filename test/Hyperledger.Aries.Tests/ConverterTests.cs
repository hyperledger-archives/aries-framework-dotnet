using System.Linq;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public class ConverterTests
    {
        [Fact]
        public void SerializeAgentMessageWithDecorators()
        {
            var message = new ConnectionRequestMessage();
            message.AddDecorator(new SampleDecorator() { Prop1 = "123" }, "sample");

            var serialized = message.ToJson();

            var token = JObject.Parse(serialized);

            Assert.NotNull(token["~sample"]);
            Assert.Equal("123", token["~sample"]["Prop1"]);
        }

        [Fact]
        public void DeserializeAgentMessageWithDecorators()
        {
            var messageJson = "{\"@id\":\"a9f6ca12-2e36-4fed-b8d1-7a2cd6e3692d\",\"@type\":\"did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/request\",\"~sample\":{\"Prop1\":\"123\"}}";
            var obj = JsonConvert.DeserializeObject<ConnectionRequestMessage>(messageJson,
                new AgentMessageReader<ConnectionRequestMessage>());

            Assert.NotNull(obj);

            var decorator = obj.GetDecorator<SampleDecorator>("sample");

            Assert.NotNull(decorator);
            Assert.IsType<SampleDecorator>(decorator);
            Assert.Equal("123", decorator.Prop1);
        }

        [Fact(DisplayName = "Decode credential preview attribute with null mime-type")]
        public void DecodeCredentialPreviewAttribute()
        {
            var json = new { name = "some name", value = "some value" }.ToJson();

            var attr = JsonConvert.DeserializeObject<CredentialPreviewAttribute>(json);

            Assert.NotNull(attr);
            Assert.Equal("some name", attr.Name);
            Assert.Equal("some value", attr.Value);
            Assert.Null(attr.MimeType);
        }

        [Fact(DisplayName = "Deserialize agent endpoint stored as string")]
        public void DeserializeAgentEndpointFromString()
        {
            var json = new { verkey = "1" }.ToJson();

            var endpoint = json.ToObject<AgentEndpoint>();
            Assert.NotNull(endpoint.Verkey);
            Assert.NotEmpty(endpoint.Verkey);
            Assert.Equal("1", endpoint.Verkey.First());
        }

        [Fact(DisplayName = "Deserialize agent endpoint stored as string array")]
        public void DeserializeAgentEndpointFromStringArray()
        {
            var json = new { verkey = new[] { "1" } }.ToJson();

            var endpoint = json.ToObject<AgentEndpoint>();
            Assert.NotNull(endpoint.Verkey);
            Assert.NotEmpty(endpoint.Verkey);
            Assert.Equal("1", endpoint.Verkey.First());
        }

        [Fact(DisplayName = "Deserialize agent endpoint DID")]
        public void DeserializeAgentEndpointDidFromString()
        {
            var json = new { did = "1" }.ToJson();

            var endpoint = json.ToObject<AgentEndpoint>();
            Assert.Equal("1", endpoint.Did);
        }

        [Fact(DisplayName = "Convert attribute filter to correct JSON")]
        public void ConvertAttributeValueToJson()
        {
            var filter = new AttributeFilter
            {
                SchemaId = "1",
                AttributeValue = new AttributeValue
                {
                    Name = "full-name",
                    Value = "alice"
                }
            };

            var json = filter.ToJson();

            Assert.NotNull(json);
            Assert.Equal("{\"schema_id\":\"1\",\"attr::full-name::value\":\"alice\"}", json);
        }

        [Fact(DisplayName = "Convert JSON to attribute filter")]
        public void ConvertJsonToAttributeFilter()
        {
            var json = "{\"schema_id\":\"1\",\"attr::full name::value\":\"alice\"}";
            var filter = json.ToObject<AttributeFilter>();

            Assert.NotNull(filter);
            Assert.NotNull(filter.AttributeValue);
            Assert.Equal("full name", filter.AttributeValue.Name);
            Assert.Equal("alice", filter.AttributeValue.Value);
            Assert.Equal("1", filter.SchemaId);
            Assert.Null(filter.CredentialDefinitionId);
        }
    }

    class SampleDecorator
    {
        public string Prop1 { get; set; }
    }
}