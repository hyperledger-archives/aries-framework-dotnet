using AgentFramework.Core.Models.Dids;
using Newtonsoft.Json;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class DidDocTests
    {
        [Fact]
        public void CanDeserializeDidDoc()
        {
            var jsonDidDoc = "{\r\n    " +
                                "   \"@context\": \"https://w3id.org/did/v1\",\r\n    " +
                                "   \"publicKey\": [\r\n      " +
                                "   {\r\n        " +
                                "       \"id\": \"did:sov:8fgY2SmLQBwcsqZ8tMHRre#keys-1\",\r\n        " +
                                "       \"type\": \"Ed25519VerificationKey2018\",\r\n        " +
                                "       \"controller\": \"did:sov:8fgY2SmLQBwcsqZ8tMHRre\",\r\n        " +
                                "       \"publicKeyBase58\": \"5BNGGHwZ5oserT2UvDS7BmqNp5W3edNsGdm3DRHDynr5\"\r\n      " +
                                "   }\r\n],\r\n    " +
                                "   \"service\": [\r\n      " +
                                "   {\r\n        " +
                                "       \"id\": \"did:sov:8fgY2SmLQBwcsqZ8tMHRre;indy\",\r\n        " +
                                "       \"type\": \"IndyAgent\",\r\n        " +
                                "       \"recipientKeys\": [],\r\n        " +
                                "       \"routingKeys\": [\r\n          " +
                                "       \"F9fVjYubb6HJ8tj13eLzf9wcaekhswybsVoUC4tagvhm\"\r\n],\r\n        " +
                                "       \"serviceEndpoint\": \"http://localhost:5000/agent\"\r\n      " +
                                "   }\r\n]\r\n  }";

            var result = JsonConvert.DeserializeObject<DidDoc>(jsonDidDoc);

            Assert.True(result.Context == "https://w3id.org/did/v1");
            Assert.True(result.Keys.Count == 1);
            Assert.True(result.Services.Count == 1);
        }

        [Fact]
        public void CanDeserializeDidDocWithoutServices()
        {
            var jsonDidDoc = "{\r\n    " +
                                "   \"@context\": \"https://w3id.org/did/v1\",\r\n    " +
                                "   \"publicKey\": [\r\n      " +
                                "   {\r\n        " +
                                "       \"id\": \"did:sov:8fgY2SmLQBwcsqZ8tMHRre#keys-1\",\r\n        " +
                                "       \"type\": \"Ed25519VerificationKey2018\",\r\n        " +
                                "       \"controller\": \"did:sov:8fgY2SmLQBwcsqZ8tMHRre\",\r\n        " +
                                "       \"publicKeyBase58\": \"5BNGGHwZ5oserT2UvDS7BmqNp5W3edNsGdm3DRHDynr5\"\r\n      " +
                                "   }\r\n],\r\n    " +
                                "   \"service\": []\r\n  }";

            var result = JsonConvert.DeserializeObject<DidDoc>(jsonDidDoc);

            Assert.True(result.Context == "https://w3id.org/did/v1");
            Assert.True(result.Keys.Count == 1);
            Assert.True(result.Services.Count == 0);
        }
    }
}
