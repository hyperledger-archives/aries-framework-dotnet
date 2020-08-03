//namespace SendPingRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Connections;

//  public class Validate_Should
//  {
//    private SendPingRequestValidator SendPingRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new SendPingRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = SendPingRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => SendPingRequestValidator
//      .ShouldHaveValidationErrorFor(aSendPingRequest => aSendPingRequest.Days, -1);

//    public void Setup() => SendPingRequestValidator = new SendPingRequestValidator();
//  }
//}
