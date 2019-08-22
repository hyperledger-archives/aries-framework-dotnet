using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Models.EphemeralChallenge;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stateless;

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Represents an ephemeral challenge record in the agents wallet.
    /// </summary>
    /// <seealso cref="RecordBase" />
    public class EphemeralChallengeRecord : RecordBase
    {
        private ChallengeState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="EphemeralChallengeRecord"/> class.
        /// </summary>
        public EphemeralChallengeRecord()
        {
            State = ChallengeState.Challenged;
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="EphemeralChallengeRecord"/>
        /// </summary>
        /// <returns></returns>
        public EphemeralChallengeRecord ShallowCopy()
        {
            return (EphemeralChallengeRecord)MemberwiseClone();
        }


        /// <summary>
        /// Creates a deep copy of the current <see cref="EphemeralChallengeRecord"/>
        /// </summary>
        /// <returns></returns>
        public EphemeralChallengeRecord DeepCopy()
        {
            var copy = (EphemeralChallengeRecord)MemberwiseClone();
            copy.Challenge = Challenge;
            copy.Response = Response;
            copy.Tags = new Dictionary<string, string>(Tags);
            return copy;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        public override string TypeName => "AF.EphemeralChallengeRecord";

        /// <summary>
        /// Gets or sets the challenge.
        /// </summary>
        /// <value>The challenge.</value>
        public EphemeralChallengeContents Challenge
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the challenge response.
        /// </summary>
        /// <value>The challenge response.</value>
        public EphemeralChallengeContents Response
        {
            get;
            set;
        }

        #region State Machine Implementation

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public ChallengeState State
        {
            get => _state;
            set => Set(value, ref _state);
        }

        /// <summary>
        /// Triggers the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="trigger">Trigger.</param>
        public Task TriggerAsync(ChallengeTrigger trigger) => GetStateMachine().FireAsync(trigger);

        private StateMachine<ChallengeState, ChallengeTrigger> GetStateMachine()
        {
            var state = new StateMachine<ChallengeState, ChallengeTrigger>(() => State, x => State = x);
            state.Configure(ChallengeState.Challenged).Permit(ChallengeTrigger.AcceptChallenge, ChallengeState.Accepted);
            state.Configure(ChallengeState.Challenged).Permit(ChallengeTrigger.RejectChallenge, ChallengeState.Rejected);
            state.Configure(ChallengeState.Challenged).Permit(ChallengeTrigger.InvalidChallengeResponse, ChallengeState.Invalid);
            return state;
        }

        #endregion
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"State={State}, " +
            $"Challenge={Challenge}, " +
            $"Response={Response}, " +
            base.ToString();  
    }

    /// <summary>
    /// Enumeration of possible connection states
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChallengeState
    {
        /// <summary>
        /// Challenged state
        /// </summary>
        Challenged = 0,
        /// <summary>
        /// Challenge accepted state
        /// </summary>
        Accepted,
        /// <summary>
        /// Challenge rejected state
        /// </summary>
        Rejected,
        /// <summary>
        /// Challenge response was invalid
        /// </summary>
        Invalid
    }

    /// <summary>
    /// Enumeration of possible triggers that change the connections state
    /// </summary>
    public enum ChallengeTrigger
    {
        /// <summary>
        /// Accept challenge trigger
        /// </summary>
        AcceptChallenge,
        /// <summary>
        /// Reject challenge trigger
        /// </summary>
        RejectChallenge,
        /// <summary>
        /// The challenge response was invalid
        /// </summary>
        InvalidChallengeResponse
    }
}
