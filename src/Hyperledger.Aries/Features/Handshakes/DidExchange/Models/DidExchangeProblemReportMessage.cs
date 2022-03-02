using System;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Handshakes.DidExchange.Models
{
    public class DidExchangeProblemReportMessage : AgentMessage
    {
        
        // Bookmark: Serialize via enum
        public const string RequestNotAccepted = "request_not_accepted";

        public const string RequestProcessingError = "request_processing_error";

        public const string ResponseNotAccepted = "response_not_accepted";

        public const string ResponseProcessingError = "response_processing_error";
        
        public DidExchangeProblemReportMessage(string error = RequestNotAccepted) : base(true)
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.DidExchange.ProblemReport;
            ProblemCode = error;
        }
        
        [JsonProperty("problem-code")]
        public string ProblemCode { get; set; }
        
        [JsonProperty("explain")]
        public string Explain { get; set; }
        
        public enum Error
        {
            RequestNotAccepted,
            RequestProcessingError,
            ResponseNotAccepted,
            ResponseProcessingError
        }
    }
}
