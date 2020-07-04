﻿namespace Hyperledger.Aries.OpenApi.Features.Credentials
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Hyperledger.Aries.OpenApi.Features.IssueCredentials;
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;

  internal partial class CredentialState
  {
    public class OfferCredentialHandler : BaseHandler<OfferCredentialAction>
    {
      private readonly HttpClient HttpClient;

      public OfferCredentialHandler
      (
        IStore aStore,
        HttpClient aHttpClient
      ) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        OfferCredentialAction aOfferCredentialAction,
        CancellationToken aCancellationToken
      )
      {
        OfferCredentialRequest offerCredentialRequest =
          aOfferCredentialAction.OfferCredentialRequest;

        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<OfferCredentialRequest>
          (
            offerCredentialRequest.GetRoute(),
            offerCredentialRequest
          );

        httpResponseMessage.EnsureSuccessStatusCode();

        return Unit.Value;
      }
    }
  }
}
