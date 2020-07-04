namespace Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure
{
  using Hyperledger.Aries.OpenApi.Features.Credentials;
  using FluentAssertions;

  public partial class BaseTest
  {
    internal static void ValidateGetCredentialsResponse
    (
      GetCredentialsRequest aGetCredentialsRequest, 
      GetCredentialsResponse aGetCredentialsResponse
    )
    {
      aGetCredentialsResponse.CorrelationId.Should().Be(aGetCredentialsRequest.CorrelationId);
      aGetCredentialsResponse.CredentialRecords.Should().NotBeNull();
      aGetCredentialsResponse.CredentialRecords.Count.Should().Be(1);
    }

  }
}
