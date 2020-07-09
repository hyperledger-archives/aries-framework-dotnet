namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using FluentValidation;
  
  public class ReceiveInvitationRequestValidator : AbstractValidator<ReceiveInvitationRequest>
  {

    public ReceiveInvitationRequestValidator()
    {
      RuleFor(aReceiveInvitationRequest => aReceiveInvitationRequest.InvitationDetails)
        .NotEmpty();
    }
  }
}