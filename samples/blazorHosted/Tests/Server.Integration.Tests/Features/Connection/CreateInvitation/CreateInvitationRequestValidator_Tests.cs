namespace CreateInvitationRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using BlazorHosted.Features.Connections;
  using Hyperledger.Aries.Features.DidExchange;
  using System.Linq;

  public class Validate_Should
  {
    private CreateInvitationRequestValidator CreateInvitationRequestValidator { get; set; }
    private CreateInvitationRequest CreateInvitationRequest { get; set; }

    public Validate_Should()
    {
      CreateInvitationRequestValidator = new CreateInvitationRequestValidator();
      
      // Set Valid values here
      var inviteConfiguration = new InviteConfiguration
      {
        AutoAcceptConnection = true,
      };

      inviteConfiguration.MyAlias.Name = "Faber";
      CreateInvitationRequest = new CreateInvitationRequest(inviteConfiguration);
    }

    public void Be_Valid()
    {
      ValidationResult validationResult = CreateInvitationRequestValidator.TestValidate(CreateInvitationRequest);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_InvitationConfiguration_is_null()
    {
      CreateInvitationRequest.InviteConfiguration = null;

      ValidationResult validationResult = CreateInvitationRequestValidator.TestValidate(CreateInvitationRequest);
      validationResult.Errors.Count.Should().BeGreaterOrEqualTo(1);
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
          $"{nameof(CreateInvitationRequest.InviteConfiguration.AutoAcceptConnection)}");
    }
  }
}
