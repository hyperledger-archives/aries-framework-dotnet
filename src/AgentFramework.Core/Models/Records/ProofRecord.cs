using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stateless;

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Represents a proof record in the agency wallet
    /// </summary>
    /// <seealso cref="RecordBase" />
    public class ProofRecord : RecordBase
    {
        private ProofState _state;

        /// <summary>Initializes a new instance of the <see cref="ProofRecord"/> class.</summary>
        public ProofRecord()
        {
            State = ProofState.Requested;
        }

        /// <summary>Creates a shallow copy of the current <see cref="ProofRecord"/> object.</summary>
        /// <returns></returns>
        public ProofRecord ShallowCopy()
        {
            return (ProofRecord)MemberwiseClone();
        }

        /// <summary>Creates a deep copy of the current <see cref="ProofRecord"/> object.</summary>
        /// <returns></returns>
        public ProofRecord DeepCopy()
        {
            var proof = (ProofRecord)MemberwiseClone();
            proof.Tags = new Dictionary<string, string>(Tags);
            return proof;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        public override string TypeName => "AF.ProofRecord";

        /// <summary>
        /// Gets or sets the proof request json.
        /// </summary>
        /// <value>The proof request json.</value>
        public string RequestJson
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the proof json.
        /// </summary>
        /// <value>The proof json.</value>
        public string ProofJson
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the connection identifier associated with this proof request.
        /// </summary>
        /// <value>The connection identifier.</value>
        [JsonIgnore]
        public string ConnectionId
        {
            get => Get();
            set => Set(value);
        }

        #region State Machine Implementation

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public ProofState State
        {
            get => _state;
            set => Set(value, ref _state);
        }

        /// <summary>
        /// Triggers the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="trigger">Trigger.</param>
        public Task TriggerAsync(ProofTrigger trigger) => GetStateMachine().FireAsync(trigger);

        private StateMachine<ProofState, ProofTrigger> GetStateMachine()
        {
            var state = new StateMachine<ProofState, ProofTrigger>(() => State, x => State = x);
            state.Configure(ProofState.Requested).Permit(ProofTrigger.Accept, ProofState.Accepted);
            state.Configure(ProofState.Requested).Permit(ProofTrigger.Reject, ProofState.Rejected);
            return state;
        }
        #endregion
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"State={State}, " +
            $"ConnectionId={ConnectionId}, " +
            $"RequestJson={(RequestJson?.Length > 0 ? "[hidden]" : null)}, " +
            $"ProofJson={(ProofJson?.Length > 0 ? "[hidden]" : null)}, " +
            base.ToString(); 
    }

    /// <summary>
    /// Enumeration of possible proof states
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProofState
    {
        /// <summary>
        /// The requested
        /// </summary>
        Requested = 0,
        /// <summary>
        /// The accepted
        /// </summary>
        Accepted = 1,
        /// <summary>
        /// The rejected
        /// </summary>
        Rejected = 2
    }

    /// <summary>
    /// Enumeration of possible triggers that change the proofs state
    /// </summary>
    public enum ProofTrigger
    {
        /// <summary>
        /// The request
        /// </summary>
        Request,
        /// <summary>
        /// The accept
        /// </summary>
        Accept,
        /// <summary>
        /// The reject
        /// </summary>
        Reject
    }
}