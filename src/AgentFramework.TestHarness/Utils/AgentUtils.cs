using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.TestHarness.Utils
{
    public class AgentUtils
    {
        public static async Task<AgentContext> Create(string config, string credentials, bool withPool = false, IList<MessageType> supportedMessageTypes = null)
        {
            try
            {
                await Wallet.CreateWalletAsync(config, credentials);
            }
            catch (WalletExistsException)
            {
                // OK
            }

            if (supportedMessageTypes == null)
                supportedMessageTypes = GetDefaultMessageTypes();

            return new AgentContext {
                Wallet = await Wallet.OpenWalletAsync(config, credentials),
                Pool = withPool ? new PoolAwaitable(PoolUtils.GetPoolAsync) : PoolAwaitable.FromPool(null),
                SupportedMessages = supportedMessageTypes };
        }

        public static IList<MessageType> GetDefaultMessageTypes()
        {
            return new List<MessageType>
            {
                //Connection Protocol
                new MessageType(MessageTypes.ConnectionInvitation),
                new MessageType(MessageTypes.ConnectionRequest),
                new MessageType(MessageTypes.ConnectionResponse),

                //Credential Protocol
                new MessageType(MessageTypes.IssueCredentialNames.OfferCredential),
                new MessageType(MessageTypes.IssueCredentialNames.PreviewCredential),
                new MessageType(MessageTypes.IssueCredentialNames.RequestCredential),
                new MessageType(MessageTypes.IssueCredentialNames.IssueCredential),

                //Proof protocol
                new MessageType(MessageTypes.PresentProofNames.RequestPresentation),
                new MessageType(MessageTypes.PresentProofNames.Presentation),

                //Trust ping protocol
                new MessageType(MessageTypes.TrustPingMessageType),
                new MessageType(MessageTypes.TrustPingResponseMessageType),
            };
        }

        public static Task<AgentContext> CreateRandomAgent(bool withPool= false)
        {
            return Create($"{{\"id\":\"{Guid.NewGuid()}\"}}", "{\"key\":\"test_wallet_key\"}", withPool);
        }
    }
}
