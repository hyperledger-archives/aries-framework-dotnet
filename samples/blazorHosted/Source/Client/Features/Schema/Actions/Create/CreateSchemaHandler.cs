namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using BlazorState;
  using MediatR;
  using Newtonsoft.Json;
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;

  internal partial class SchemaState
  {
    public class CreateSchemaHandler : BaseHandler<CreateSchemaAction>
    {
      private readonly HttpClient HttpClient;

      public CreateSchemaHandler
      (
        IStore aStore,
        HttpClient aHttpClient
      ) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        CreateSchemaAction aCreateSchemaAction,
        CancellationToken aCancellationToken
      )
      {
        var createInvitationRequest = new CreateSchemaRequest
        {
          Name = "degree-schema",
          Version = "1.0",
          AttributeNames = new List<string>() { "name", "date", "degree", "age", "timestamp" }
        };

        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<CreateSchemaRequest>
          (
            createInvitationRequest.GetRoute(),
            createInvitationRequest
          );

        httpResponseMessage.EnsureSuccessStatusCode();
        //string json = await httpResponseMessage.Content.ReadAsStringAsync();

        //CreateSchemaResponse createSchemaResponse =
        //  JsonConvert.DeserializeObject<CreateSchemaResponse>(json);

        return Unit.Value;
      }
    }
  }
}
