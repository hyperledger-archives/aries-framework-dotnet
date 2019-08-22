using AgentFramework.Core.Decorators.Attachments;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.Connections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AgentFramework.Core.Tests.Decorators
{
    public class AttachmentDecoratorTests
    {
        [Fact]
        public void ExtractAttachDecorator()
        {
            var json = "{\"@id\":\"123\",\"@type\":\"did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/request\",\"~attach\":[]}";

            var message = JsonConvert.DeserializeObject<ConnectionRequestMessage>(json,
                    new AgentMessageReader<ConnectionRequestMessage>());

            var decorator = message.GetDecorator<AttachDecorator>("attach");

            Assert.NotNull(decorator);
        }

        [Fact]
        public void ExtractAttachDecoratorReturnsNull()
        {
            var json = "{\"@id\":\"123\",\"@type\":\"did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/request\"}";

            var message = JsonConvert.DeserializeObject<ConnectionRequestMessage>(json,
                new AgentMessageReader<ConnectionRequestMessage>());

            var decorator = message.FindDecorator<AttachDecorator>("attach");

            Assert.Null(decorator);
        }

        [Fact]
        public void ExtractDecoratorAndAttachment()
        {
            var message = new ConnectionRequestMessage();
            message.AddAttachment(new Attachment {Nickname = "file1"});

            var jobj = JObject.Parse(message.ToJson());

            Assert.NotNull(jobj["~attach"]);
            Assert.Equal("file1", jobj["~attach"].First["nickname"]);
        }

        [Fact]
        public void GetAttachmentFromDecorator()
        {
            var json = "{\"@id\":\"1\",\"@type\":\"did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/request\",\"~attach\":[{\"nickname\":\"file1\"}]}";

            var message = JsonConvert.DeserializeObject<ConnectionRequestMessage>(json,
                new AgentMessageReader<ConnectionRequestMessage>());
            var decorator = message.GetDecorator<AttachDecorator>("attach");

            Assert.NotNull(decorator);

            var file = message.GetAttachment("file1");
            Assert.NotNull(file);

            var file2 = message.GetAttachment("invalid");
            Assert.Null(file2);
        }

        [Fact]
        public void RemoveAttachmentFromMessage()
        {
            var json = "{\"@id\":\"1\",\"@type\":\"did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/request\",\"~attach\":[{\"nickname\":\"file1\"}]}";

            var message = JsonConvert.DeserializeObject<ConnectionRequestMessage>(json,
                new AgentMessageReader<ConnectionRequestMessage>());
            var decorator = message.GetDecorator<AttachDecorator>("attach");

            Assert.NotNull(decorator);

            var file = message.GetAttachment("file1");
            Assert.NotNull(file);

            message.RemoveAttachment("file1");

            var file2 = message.GetAttachment("file1");
            Assert.Null(file2);
        }
    }
}
