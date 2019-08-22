using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Models.Connections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stateless;

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Represents a connection record in the agency wallet.
    /// </summary>
    /// <seealso cref="RecordBase" />
    public class ConnectionRecord : RecordBase
    {
        private ConnectionState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionRecord"/> class.
        /// </summary>
        public ConnectionRecord()
        {
            State = ConnectionState.Invited;
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="ConnectionRecord"/>
        /// </summary>
        /// <returns></returns>
        public ConnectionRecord ShallowCopy()
        {
            return (ConnectionRecord)MemberwiseClone();
        }


        /// <summary>
        /// Creates a deep copy of the current <see cref="ConnectionRecord"/>
        /// </summary>
        /// <returns></returns>
        public ConnectionRecord DeepCopy()
        {
            var copy = (ConnectionRecord)MemberwiseClone();
            copy.Alias = new ConnectionAlias(Alias);
            copy.Endpoint = new AgentEndpoint(Endpoint);
            copy.Tags = new Dictionary<string, string>(Tags);
            return copy;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        public override string TypeName => "AF.ConnectionRecord";

        /// <summary>
        /// Gets or sets my did.
        /// </summary>
        /// <value>My did.</value>
        [JsonIgnore]
        public string MyDid
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets my verkey
        /// </summary>
        /// <value>My vk.</value>
        [JsonIgnore]
        public string MyVk
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets their did.
        /// </summary>
        /// <value>Their did.</value>
        [JsonIgnore]
        public string TheirDid
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets their verkey.
        /// </summary>
        /// <value>Their vk.</value>
        [JsonIgnore]
        public string TheirVk
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets whether the invitation is multi-party.
        /// </summary>
        /// <value>Indicates if the property is multi-party.</value>
        [JsonIgnore]
        public bool MultiPartyInvitation
        {
            get => GetBool();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the alias associated to the connection.
        /// </summary>
        /// <value>The connection alias.</value>
        public ConnectionAlias Alias
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
        public AgentEndpoint Endpoint
        {
            get;
            set;
        }

        #region State Machine Implementation

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public ConnectionState State
        {
            get => _state;
            set => Set(value, ref _state);
        }

        /// <summary>
        /// Triggers the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="trigger">Trigger.</param>
        public Task TriggerAsync(ConnectionTrigger trigger) => GetStateMachine().FireAsync(trigger);

        private StateMachine<ConnectionState, ConnectionTrigger> GetStateMachine()
        {
            var state = new StateMachine<ConnectionState, ConnectionTrigger>(() => State, x => State = x);
            state.Configure(ConnectionState.Invited).Permit(ConnectionTrigger.InvitationAccept, ConnectionState.Negotiating);
            state.Configure(ConnectionState.Invited).Permit(ConnectionTrigger.Request, ConnectionState.Negotiating);
            state.Configure(ConnectionState.Negotiating).Permit(ConnectionTrigger.Request, ConnectionState.Connected);
            state.Configure(ConnectionState.Negotiating).Permit(ConnectionTrigger.Response, ConnectionState.Connected);
            return state;
        }

        #endregion
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"State={State}, " +
            $"MyDid={MyDid}, " +
            $"MyVk={(MyVk?.Length > 0 ? "[hidden]" : null)}, " +
            $"TheirDid={TheirDid}, " +
            $"TheirVk={(TheirVk?.Length > 0 ? "[hidden]" : null)}, " +
            $"MultiPartyInvitation={MultiPartyInvitation}, " +
            $"Alias={Alias}, " +
            $"Endpoint={Endpoint}, " +
            base.ToString();  
    }

    /// <summary>
    /// Enumeration of possible connection states
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConnectionState
    {
        /// <summary>
        /// The invited
        /// </summary>
        Invited = 0,
        /// <summary>
        /// The negotiating
        /// </summary>
        Negotiating,
        /// <summary>
        /// The connected
        /// </summary>
        Connected
    }

    /// <summary>
    /// Enumeration of possible triggers that change the connections state
    /// </summary>
    public enum ConnectionTrigger
    {
        /// <summary>
        /// The invitation accept
        /// </summary>
        InvitationAccept,
        /// <summary>
        /// The request
        /// </summary>
        Request,
        /// <summary>
        /// The response
        /// </summary>
        Response
    }
}
