using Hyperledger.Aries.Storage;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Features.OperationCompleted.Models
{
    public class OperationCompletedRecord : RecordBase
    {
        public override string TypeName => "OperationCompleted";
        public string Comment { get; set; }

        [JsonIgnore]
        public string ConnectionId
        {
            get => Get();
            set => Set(value);
        }
    }
}
