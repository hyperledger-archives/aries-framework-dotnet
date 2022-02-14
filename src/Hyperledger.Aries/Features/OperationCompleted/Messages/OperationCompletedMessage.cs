using Hyperledger.Aries.Agents;
using System;

namespace Hyperledger.Aries.Features.OperationCompleted.Messages
{
    public class OperationCompletedMessage: AgentMessage
    {
        public string Comment { get; set; }

        public OperationCompletedMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.OperationCompleted;
        }
    }
}
