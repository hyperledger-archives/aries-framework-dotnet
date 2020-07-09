//namespace AcceptInvitationRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Connections;

//  public class Validate_Should
//  {
//    private AcceptInvitationRequestValidator AcceptInvitationRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new AcceptInvitationRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = AcceptInvitationRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => AcceptInvitationRequestValidator
//      .ShouldHaveValidationErrorFor(aAcceptInvitationRequest => aAcceptInvitationRequest.Days, -1);

//    public void Setup() => AcceptInvitationRequestValidator = new AcceptInvitationRequestValidator();
//  }
//}
