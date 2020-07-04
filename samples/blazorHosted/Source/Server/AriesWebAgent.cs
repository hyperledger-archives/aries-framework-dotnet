﻿namespace Hyperledger.Aries.OpenApi
{
  using Hyperledger.Aries.Agents;
  using System;
  public class AriesWebAgent : AgentBase
  {
    public AriesWebAgent(IServiceProvider aServiceProvider) : base(aServiceProvider) { }

    protected override void ConfigureHandlers()
    {
      AddConnectionHandler();
      AddForwardHandler();
      //AddHandler<BasicMessageHandler>();
      //AddHandler<TrustPingMessageHandler>();
      //AddDiscoveryHandler();
      //AddTrustPingHandler();
      AddCredentialHandler();
      AddProofHandler();
    }
  }
}
