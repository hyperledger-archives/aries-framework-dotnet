namespace ReceiveInvitationRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;

  public class Validate_Should: BaseTest
  {
    private ReceiveInvitationRequestValidator ReceiveInvitationRequestValidator { get; set; }
    private ReceiveInvitationRequest ReceiveInvitationRequest { get; set; }

    public Validate_Should
    (
      AliceWebApplicationFactory aAliceWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aAliceWebApplicationFactory, aJsonSerializerSettings)
    {
      ReceiveInvitationRequestValidator = new ReceiveInvitationRequestValidator();
      ReceiveInvitationRequest = CreateValidReceiveInvitationRequest();
    }

    public void Be_Valid()
    {
      ValidationResult validationResult = ReceiveInvitationRequestValidator.TestValidate(ReceiveInvitationRequest);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_InvitationDetails_is_empty()
    {
      ReceiveInvitationRequest.InvitationDetails = "";
      ReceiveInvitationRequestValidator
        .ShouldHaveValidationErrorFor
        (
          aReceiveInvitationRequest => aReceiveInvitationRequest.InvitationDetails,
          ReceiveInvitationRequest
        );
    }

    public void Have_error_when_InvitationDetails_is_null()
    {
      ReceiveInvitationRequest.InvitationDetails = null;
      ReceiveInvitationRequestValidator
        .ShouldHaveValidationErrorFor
        (
          aReceiveInvitationRequest => aReceiveInvitationRequest.InvitationDetails,
          ReceiveInvitationRequest
        );
    }
  }
}
