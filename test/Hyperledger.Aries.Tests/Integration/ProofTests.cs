﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Hyperledger.Indy.AnonCredsApi;
using Xunit;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Tests.Integration
{
    public class ProofTests : IAsyncLifetime
    {
        static ProofTests()
        {
            global::Hyperledger.Aries.Utils.Runtime.SetFlags(Hyperledger.Aries.Utils.Runtime.LedgerLookupRetryFlag);
        }

        WalletConfiguration config1 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletConfiguration config2 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletConfiguration config3 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletCredentials cred = new WalletCredentials { Key = "2" };

        private MockAgent _issuerAgent;
        private MockAgent _holderAgent;
        private MockAgent _requestorAgent;
        private readonly MockAgentRouter _router = new MockAgentRouter();

        public async Task InitializeAsync()
        {
            _issuerAgent = await MockUtils.CreateAsync("issuer", config1, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)), TestConstants.StewartDid);
            _router.RegisterAgent(_issuerAgent);
            _holderAgent = await MockUtils.CreateAsync("holder", config2, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)));
            _router.RegisterAgent(_holderAgent);
            _requestorAgent = await MockUtils.CreateAsync("requestor", config3, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)));
            _router.RegisterAgent(_requestorAgent);
        }

        [Fact]
        public async Task CanPerformProofProtocol()
        {
            (var issuerConnection, var holderConnection)  = await AgentScenarios.EstablishConnectionAsync(_issuerAgent, _holderAgent);

            await AgentScenarios.IssueCredentialAsync(_issuerAgent, _holderAgent, issuerConnection, holderConnection, new List<CredentialPreviewAttribute>
            {
                new CredentialPreviewAttribute("first_name", "Test"),
                new CredentialPreviewAttribute("last_name", "Holder")
            });

            (var holderRequestorConnection, var requestorConnection) = await AgentScenarios.EstablishConnectionAsync(_holderAgent, _requestorAgent);

            await AgentScenarios.ProofProtocolAsync(_requestorAgent, _holderAgent, requestorConnection,
                holderRequestorConnection, new ProofRequest()
                {
                    Name = "ProofReq",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        {"first-name-requirement", new ProofAttributeInfo {Name = "first_name"}}
                    }
                });
        }

        public async Task DisposeAsync()
        {
            await _issuerAgent.Dispose();
            await _holderAgent.Dispose();
        }
    }
}
