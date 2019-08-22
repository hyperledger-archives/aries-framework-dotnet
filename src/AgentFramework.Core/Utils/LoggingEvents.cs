namespace AgentFramework.Core.Utils
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class LoggingEvents
    {

        //Credential events
        public const int CreateCredentialOffer = 1000;
        public const int SendCredentialOffer = 1001;
        public const int StoreCredentialRequest = 1002;

        //Proof events
        public const int GetProofRecord = 2000;
        public const int CreateProofRequest = 2001;
        public const int SendProofRequest = 2002;
        public const int StoreProofRequest = 2003;


        // Connection events
        public const int CreateInvitation = 4000;
        public const int AcceptInvitation = 4001;
        public const int ProcessConnectionRequest = 4002;
        public const int AcceptConnectionRequest = 4003;
        public const int AcceptConnectionResponse = 4004;
        public const int GetConnection = 4010;
        public const int ListConnections = 4011;
        public const int DeleteConnection = 4012;


        public const int SendMessage = 3000;
        public const int ForwardMessage = 3001;

        //Ephemeral challenge events
        public const int ListChallengeConfigurations = 5000;
        public const int GetChallengeConfiguration = 5001;
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
