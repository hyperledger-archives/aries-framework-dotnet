using System.Collections.Generic;
using System.Linq;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Common.Dids;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public class DidCommServiceTests
    {
        [Fact]
        public void CanDeserializeDidCommServiceEndpoint()
        {
            const string service = @"{
                ""id"": ""did:example:123456789abcdefghi#did-communication"",
                ""type"": ""did-communication"",
                ""priority"" : 0,
                ""recipientKeys"" : [ ""did:example:123456789abcdefghi#1"" ],
                ""routingKeys"" : [ ""did:example:123456789abcdefghi#1"" ],
                ""accept"": [
                    ""didcomm/aip2;env=rfc587"",
                    ""didcomm/aip2;env=rfc19""
                ],
                ""serviceEndpoint"": ""https://agent.example.com/""
            }";

            var obj = service.ToObject<DidCommServiceEndpoint>();
            
            Assert.NotNull(obj);
            Assert.Equal("did:example:123456789abcdefghi#did-communication", obj.Id);
            Assert.Equal("did-communication", obj.Type);
            Assert.Equal(0, obj.Priority);
            Assert.Equal("did:example:123456789abcdefghi#1", obj.RecipientKeys.First());
            Assert.Equal("did:example:123456789abcdefghi#1", obj.RoutingKeys.First());
            Assert.Equal("didcomm/aip2;env=rfc587", obj.Accept.First());
            Assert.Equal("didcomm/aip2;env=rfc19", obj.Accept.Last());
            Assert.Equal("https://agent.example.com/", obj.ServiceEndpoint);
        }
        
        [Fact]
        public void CanSerializeDidCommServiceEndpoint()
        {
            var service = new DidCommServiceEndpoint
            {
                Id = "did:example:123456789abcdefghi#did-communication",
                Priority = 0,
                RecipientKeys = new List<string>() {"did:example:123456789abcdefghi#1"},
                RoutingKeys = new List<string>() {"did:example:123456789abcdefghi#1"},
                Accept = new List<string>() {"didcomm/aip2;env=rfc587", "didcomm/aip2;env=rfc19"},
                ServiceEndpoint = "https://agent.example.com/"
            };

            var json = service.ToJson();
            
            Assert.False(string.IsNullOrEmpty(json));

            var jobj = JObject.Parse(json);
            
            Assert.Equal("https://agent.example.com/", jobj["serviceEndpoint"]);
        }

        [Fact]
        public void CanDeriveDidCommServiceEndpointFromConnectionAndProvisioningRecord()
        {
            var connectionRecord = new ConnectionRecord
            {
                MyDid = "did:example:123456789abcdefghi",
                MyVk = "8HH5gYEeNc3z7PYXmd54d4x6qAfCNrqQqEB3nS7Zfu7K"
            };

            var provisioningRecord = new ProvisioningRecord
            {
                Endpoint = new AgentEndpoint
                {
                    Verkey = new []{"did:sov:123456789abcdefghi"}
                }
            };

            var result = connectionRecord.MyDidCommService(provisioningRecord);
            
            Assert.NotNull(result);
            Assert.Equal("did:key:z6MkmjY8GnV5i9YTDtPETC2uUAW6ejw3nk5mXF5yci5ab7th", result.RecipientKeys.First());
            Assert.Equal("did:sov:123456789abcdefghi", result.RoutingKeys.First());
        }
    }
}
