//namespace Hyperledger.Aries.AspNetCore.Features.Credentials
//{
//  using Hyperledger.Aries.AspNetCore.Features.Bases;
//  using BlazorState;
//  using MediatR;
//  using System;
//  using System.Collections.Generic;
//  using System.Net.Http;
//  using System.Net.Http.Json;
//  using System.Threading;
//  using System.Threading.Tasks;

//  internal partial class CredentialState
//  {
//    public class SaveCredentialHandler : BaseHandler<SaveCredentialAction>
//    {
//      private readonly HttpClient HttpClient;

//      public SaveCredentialHandler
//      (
//        IStore aStore,
//        HttpClient aHttpClient
//      ) : base(aStore)
//      {
//        HttpClient = aHttpClient;
//      }

//      public override async Task<Unit> Handle
//      (
//        SaveCredentialAction aSaveCredentialAction,
//        CancellationToken aCancellationToken
//      )
//      {
//        SaveCredentialRequest saveCredentialRequest = 
//          aSaveCredentialAction.SaveCredentialRequest;

//        HttpResponseMessage httpResponseMessage =
//          await HttpClient.PostAsJsonAsync<SaveCredentialRequest>
//          (
//            saveCredentialRequest.GetRoute(),
//            saveCredentialRequest
//          );

//        httpResponseMessage.EnsureSuccessStatusCode();

//        return Unit.Value;
//      }
//    }
//  }
//}
