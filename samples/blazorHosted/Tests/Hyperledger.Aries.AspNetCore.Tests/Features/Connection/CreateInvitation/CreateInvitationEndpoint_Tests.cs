namespace CreateInvitationEndpoint
{
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading.Tasks;

  public class Returns : BaseTest
  {
    private readonly CreateInvitationRequest CreateInvitationRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      CreateInvitationRequest = CreateValidCreateInvitationRequest();
    }

    public async Task CreateInvitationResponse_using_Json_Net()
    {
      CreateInvitationResponse createInvitationResponse =
        await Post(CreateInvitationRequest.GetRoute(), CreateInvitationRequest);

      ValidateCreateInvitationResponse(CreateInvitationRequest, createInvitationResponse);
    }

    public async Task CreateInvitationResponse_using_System_Text_Json()
    {
      HttpResponseMessage httpResponseMessage =
        await HttpClient
          .PostAsJsonAsync<CreateInvitationRequest>(CreateInvitationRequest.GetRoute(), CreateInvitationRequest);

      CreateInvitationResponse createInvitationResponse =
        await httpResponseMessage.Content.ReadFromJsonAsync<CreateInvitationResponse>();

      ValidateCreateInvitationResponse(CreateInvitationRequest, createInvitationResponse);
    }

    public async Task ValidationError()
    {
      // Set invalid value.
      // This is NOT to test all validation rules just to test that they are wired up.
      // If one fires they should all fire and each is tested in the Validator test
      CreateInvitationRequest.InviteConfiguration = null;

      await ConfirmEndpointValidationError
      (
        CreateInvitationRequest.GetRoute(),
        CreateInvitationRequest,
        nameof(CreateInvitationRequest.InviteConfiguration)
      );
    }
  }
}
