namespace CreateInvitationRequestValidator_
{
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using System.Linq;
  
  public class Validate_Should : BaseTest
  {
    private CreateInvitationRequest CreateInvitationRequest { get; set; }
    private CreateInvitationRequestValidator CreateInvitationRequestValidator { get; set; }

    public Validate_Should
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      CreateInvitationRequestValidator = new CreateInvitationRequestValidator();
      CreateInvitationRequest = CreateValidCreateInvitationRequest();
    }

    public void Be_Valid()
    {
      ValidationResult validationResult = CreateInvitationRequestValidator.TestValidate(CreateInvitationRequest);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_AutoAccept_is_false()
    {
      CreateInvitationRequest.InviteConfiguration.AutoAcceptConnection = false;

      ValidationResult validationResult = CreateInvitationRequestValidator.TestValidate(CreateInvitationRequest);
      validationResult.Errors.Count.Should().BeGreaterOrEqualTo(1);
      validationResult.Errors.First().PropertyName.Should()
        .Be
        (
          $"{nameof(CreateInvitationRequest.InviteConfiguration)}." +
          $"{nameof(CreateInvitationRequest.InviteConfiguration.AutoAcceptConnection)}"
        );
    }

    public void Have_error_when_InvitationConfiguration_is_null()
    {
      CreateInvitationRequest.InviteConfiguration = null;

      ValidationResult validationResult = CreateInvitationRequestValidator.TestValidate(CreateInvitationRequest);
      validationResult.Errors.Count.Should().BeGreaterOrEqualTo(1);
    }
  }
}
