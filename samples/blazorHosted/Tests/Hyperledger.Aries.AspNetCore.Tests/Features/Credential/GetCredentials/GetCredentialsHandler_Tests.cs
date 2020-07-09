namespace GetCredentialsHandler
{
  using Hyperledger.Aries.AspNetCore.Features.Credentials;
  using Hyperledger.Aries.AspNetCore.Server;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using System.Threading.Tasks;

  public class Handle_Returns : BaseTest
  {
    private readonly GetCredentialsRequest GetCredentialsRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      GetCredentialsRequest = new GetCredentialsRequest();
    }

    public async Task GetCredentialsResponse()
    {
      GetCredentialsResponse GetCredentialsResponse = await Send(GetCredentialsRequest);

      ValidateGetCredentialsResponse(GetCredentialsRequest, GetCredentialsResponse);
    }
  }
}
