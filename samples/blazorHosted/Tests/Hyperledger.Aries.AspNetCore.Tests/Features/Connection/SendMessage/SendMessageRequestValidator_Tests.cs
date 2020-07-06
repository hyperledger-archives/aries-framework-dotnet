//namespace SendMessageRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.BasicMessaging;

//  public class Validate_Should
//  {
//    private SendMessageRequestValidator SendMessageRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var sendMessageRequest = new SendMessageRequest
//      {
//        // Set Valid values here
//        Message = "Hello World"
//      };

//      ValidationResult validationResult = SendMessageRequestValidator.TestValidate(sendMessageRequest);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Message_is_empty() => SendMessageRequestValidator
//      .ShouldHaveValidationErrorFor(aSendMessageRequest => aSendMessageRequest.Message, string.Empty);

//    public void Setup() => SendMessageRequestValidator = new SendMessageRequestValidator();
//  }
//}
