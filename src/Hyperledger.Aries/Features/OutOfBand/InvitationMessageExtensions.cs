using System.Collections.Generic;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;

namespace Hyperledger.Aries.Features.OutOfBand
{
    public static class InvitationMessageExtensions
    {
        public static void AddRequests(this InvitationMessage message, IEnumerable<AgentMessage> requests)
        {
            if (requests == null) return;
            
            List<Attachment> attachments = new List<Attachment>();
            int index = 0;
            foreach (var request in requests)
            {
                attachments.Add(new Attachment
                {
                    Id = "request-" + index,
                    Data = new AttachmentContent
                    {
                        Json = request.ToJson()
                    }
                });
            }

            message.AttachedRequests = attachments.ToArray();
        }
    }
}
