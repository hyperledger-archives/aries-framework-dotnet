//namespace RecieveInvitationRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.OpenApi.Features.Connections;

//  public class Validate_Should
//  {
//    private RecieveInvitationRequestValidator RecieveInvitationRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new RecieveInvitationRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = RecieveInvitationRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => RecieveInvitationRequestValidator
//      .ShouldHaveValidationErrorFor(aRecieveInvitationRequest => aRecieveInvitationRequest.Days, -1);

//    public void Setup() => RecieveInvitationRequestValidator = new RecieveInvitationRequestValidator();
//  }
//}
