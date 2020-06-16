namespace BlazorHosted.Features.Connections
{
  using FluentValidation;

  public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
  {
    public SendMessageRequestValidator()
    {
      RuleFor(aSendMessageRequest => aSendMessageRequest.ConnectionId)
        .NotEmpty();

      RuleFor(aSendMessageRequest => aSendMessageRequest.Message)
        .NotEmpty();
    }
  }
}
