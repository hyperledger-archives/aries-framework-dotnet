namespace BlazorHosted.Features.Connections
{
  using FluentValidation;
  
  public class AcceptInvitationRequestValidator : AbstractValidator<AcceptInvitationRequest>
  {

    public AcceptInvitationRequestValidator()
    {
      RuleFor(aAcceptInvitationRequest => aAcceptInvitationRequest.InvitationDetails)
        .NotEmpty();
    }
  }
}