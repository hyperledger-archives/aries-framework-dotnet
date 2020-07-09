namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using FluentValidation;

  public class SendPingRequestValidator : AbstractValidator<SendPingRequest>
  {
    public SendPingRequestValidator()
    {
      RuleFor(aSendPingRequest => aSendPingRequest.ConnectionId)
        .NotEmpty();
    }
  }
}
