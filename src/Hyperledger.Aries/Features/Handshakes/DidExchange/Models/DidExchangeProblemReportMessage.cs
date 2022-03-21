using System;
using System.Runtime.Serialization;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hyperledger.Aries.Features.Handshakes.DidExchange.Models
{
    public class DidExchangeProblemReportMessage : AgentMessage
    {
        private const string RequestNotAccepted = "request_not_accepted";
        private const string RequestProcessingError = "request_processing_error";
        private const string ResponseNotAccepted = "response_not_accepted";
        private const string ResponseProcessingError = "response_processing_error";
        
        public DidExchangeProblemReportMessage() : base(true)
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.DidExchange.ProblemReport;
        }
        
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("problem-code")]
        public Error ProblemCode { get; set; }
        
        [JsonProperty("explain")]
        public string Explain { get; set; }
        
        public enum Error
        {
            [EnumMember(Value = DidExchangeProblemReportMessage.RequestNotAccepted)]
            RequestNotAccepted,
            [EnumMember(Value = DidExchangeProblemReportMessage.RequestProcessingError)]
            RequestProcessingError,
            [EnumMember(Value = DidExchangeProblemReportMessage.ResponseNotAccepted)]
            ResponseNotAccepted,
            [EnumMember(Value = DidExchangeProblemReportMessage.ResponseProcessingError)]
            ResponseProcessingError
        }
    }
}
