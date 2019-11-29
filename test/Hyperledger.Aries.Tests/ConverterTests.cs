using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.IssueCredential;

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
        public void DeserializeAgentMessageWithDectorators()
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
    }

    class SampleDecorator
    {
        public string Prop1 { get; set; }
    }
}