using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Decorators.Threading;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.EphemeralChallenge;
using AgentFramework.Core.Models.EphemeralChallenge;
using AgentFramework.Core.Models.Events;
using AgentFramework.Core.Models.Proofs;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using AgentFramework.Core.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public class DefaultEphemeralChallengeService : IEphemeralChallengeService
    {
        /// <summary>
        /// The event aggregator.
        /// </summary>
        protected readonly IEventAggregator EventAggregator;
        /// <summary>
        /// The proof service.
        /// </summary>
        protected readonly IProofService ProofService;
        /// <summary>
        /// The record service.
        /// </summary>
        protected readonly IWalletRecordService RecordService;
        /// <summary>
        /// The provisioning service.
        /// </summary>
        protected readonly IProvisioningService ProvisioningService;
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<DefaultEphemeralChallengeService> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEphemeralChallengeService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="proofService">The proof service.</param>
        /// <param name="recordService">The record service.</param>
        /// <param name="provisioningService">The provisioning service.</param>
        /// <param name="logger">The logger.</param>
        public DefaultEphemeralChallengeService(
            IEventAggregator eventAggregator,
            IProofService proofService,
            IWalletRecordService recordService,
            IProvisioningService provisioningService,
            ILogger<DefaultEphemeralChallengeService> logger)
        {
            EventAggregator = eventAggregator;
            ProofService = proofService;
            RecordService = recordService;
            ProvisioningService = provisioningService;
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual async Task<string> CreateChallengeConfigAsync(IAgentContext agentContext, EphemeralChallengeConfiguration config)
        {
            EphemeralChallengeConfigRecord configRecord = new EphemeralChallengeConfigRecord
            {
                Id = Guid.NewGuid().ToString(),
                Name = config.Name,
                Type = config.Type,
                Contents = JObject.FromObject(config.Contents)
            };

            await RecordService.AddAsync(agentContext.Wallet, configRecord);
            return configRecord.Id;
        }

        /// <inheritdoc />
        public virtual async Task<EphemeralChallengeConfigRecord> GetChallengeConfigAsync(IAgentContext agentContext, string configId)
        {
            Logger.LogInformation(LoggingEvents.GetChallengeConfiguration, "Configuration Id {0}", configId);

            var record = await RecordService.GetAsync<EphemeralChallengeConfigRecord>(agentContext.Wallet, configId);

            if (record == null)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, "Challenge configuration record not found");

            return record;
        }

        /// <inheritdoc />
        public virtual Task<List<EphemeralChallengeConfigRecord>> ListChallengeConfigsAsync(IAgentContext agentContext, ISearchQuery query = null,
            int count = 100)
        {
            Logger.LogInformation(LoggingEvents.ListChallengeConfigurations, "List Challenge Configurations");

            return RecordService.SearchAsync<EphemeralChallengeConfigRecord>(agentContext.Wallet, query, null, count);
        }

        /// <inheritdoc />
        public virtual async Task<EphemeralChallengeRecord> GetChallengeAsync(IAgentContext agentContext, string challengeId)
        {
            Logger.LogInformation(LoggingEvents.GetChallengeConfiguration, "Configuration Id {0}", challengeId);

            var record = await RecordService.GetAsync<EphemeralChallengeRecord>(agentContext.Wallet, challengeId);

            if (record == null)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, "Challenge record not found");

            return record;
        }

        /// <inheritdoc />
        public virtual Task<List<EphemeralChallengeRecord>> ListChallengesAsync(IAgentContext agentContext, ISearchQuery query = null,
            int count = 100)
        {
            Logger.LogInformation(LoggingEvents.ListChallengeConfigurations, "List Challenges");

            return RecordService.SearchAsync<EphemeralChallengeRecord>(agentContext.Wallet, query, null, count);
        }

        /// <inheritdoc />
        public virtual async Task<(EphemeralChallengeMessage, EphemeralChallengeRecord)> CreateChallengeAsync(IAgentContext agentContext,
            string challengeConfigId)
        {
            var config = await GetChallengeConfigAsync(agentContext, challengeConfigId);
            EphemeralChallengeRecord challengeRecord = new EphemeralChallengeRecord
            {
                Id = Guid.NewGuid().ToString()
            };
            EphemeralChallengeMessage challengeMessage = new EphemeralChallengeMessage();

            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            challengeMessage.ChallengerName = provisioning.Owner?.Name;
            challengeMessage.ChallengerImageUrl = provisioning.Owner?.ImageUrl;

            challengeMessage.ServiceEndpoint = provisioning.Endpoint.Uri;
            challengeMessage.RecipientKeys = new[] {provisioning.Endpoint.Verkey};

            if (config.Type == ChallengeType.Proof)
            {
                var proofRequestConfig = config.Contents.ToObject<ProofRequestConfiguration>();
                (var proofRequest, var _) = await ProofService.CreateProofRequestAsync(agentContext, new ProofRequest
                {
                    Name = config.Name,
                    Version = "1.0",
                    Nonce = $"0{Guid.NewGuid().ToString("N")}",
                    RequestedAttributes = proofRequestConfig.RequestedAttributes,
                    RequestedPredicates = proofRequestConfig.RequestedPredicates,
                    NonRevoked = proofRequestConfig.NonRevoked
                });
                challengeRecord.Challenge = new EphemeralChallengeContents
                {
                    Type = ChallengeType.Proof,
                    Contents = JsonConvert.DeserializeObject<JObject>(proofRequest.ProofRequestJson)
                };
                challengeMessage.Challenge = challengeRecord.Challenge;
            }

            challengeRecord.SetTag(TagConstants.Role, TagConstants.Requestor);
            challengeRecord.SetTag(TagConstants.LastThreadId, challengeMessage.Id);
            await RecordService.AddAsync(agentContext.Wallet, challengeRecord);

            return (challengeMessage,
                    challengeRecord);
        }

        /// <inheritdoc />
        public virtual async Task<ChallengeState> GetChallengeStateAsync(IAgentContext agentContext, string challengeId,
            bool deleteIfResolved = true)
        {
            var challenge = await GetChallengeAsync(agentContext, challengeId);

            if (deleteIfResolved && challenge.State != ChallengeState.Challenged)
                await RecordService.DeleteAsync<EphemeralChallengeRecord>(agentContext.Wallet, challengeId);

            return challenge.State;
        }

        /// <inheritdoc />
        public virtual async Task<string> ProcessChallengeResponseAsync(IAgentContext agentContext,
            EphemeralChallengeResponseMessage challengeResponse)
        {
            var threadId = challengeResponse.GetThreadId();

            //TODO improve this
            var results = await ListChallengesAsync(agentContext, new EqSubquery(TagConstants.LastThreadId, threadId));
            var record = results.FirstOrDefault();

            if (record == null)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, "Challenge not found");

            if (record.State != ChallengeState.Challenged)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Challenge state was invalid. Expected '{ChallengeState.Challenged}', found '{record.State}'");

            if (record.State != ChallengeState.Challenged)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, "Challenge not found");

            record.Response = challengeResponse.Response;

            if (challengeResponse.Status == EphemeralChallengeResponseStatus.Accepted)
            {
                var result = await ProofService.VerifyProofAsync(agentContext, record.Challenge.Contents.ToJson(),
                    record.Response.Contents.ToJson());
                if (result)
                    await record.TriggerAsync(ChallengeTrigger.AcceptChallenge);
                else
                    await record.TriggerAsync(ChallengeTrigger.InvalidChallengeResponse);
            }
            else
                await record.TriggerAsync(ChallengeTrigger.RejectChallenge);

            await RecordService.UpdateAsync(agentContext.Wallet, record);

            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                MessageType = MessageTypes.EphemeralChallenge,
                RecordId = record.Id,
                ThreadId = challengeResponse.GetThreadId()
            });

            return record.Id;
        }

        /// <inheritdoc />
        public virtual async Task<EphemeralChallengeResponseMessage> CreateProofChallengeResponseAsync(IAgentContext agentContext, EphemeralChallengeMessage message, RequestedCredentials credentials)
        {
            if (message.Challenge.Type != ChallengeType.Proof)
                throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Challenge.Type != Proof");

            var challengeResponse = new EphemeralChallengeResponseMessage
            {
                Id = Guid.NewGuid().ToString(),
                Status = EphemeralChallengeResponseStatus.Accepted
            };

            var proofRequest = message.Challenge.Contents.ToObject<ProofRequest>();

            var proof = await ProofService.CreateProofAsync(agentContext, proofRequest, credentials);
            challengeResponse.Response = new EphemeralChallengeContents
            {
                Type = ChallengeType.Proof,
                Contents = JsonConvert.DeserializeObject<JObject>(proof)
            };

            challengeResponse.ThreadFrom(message);

            return challengeResponse;
        }
    }
}
