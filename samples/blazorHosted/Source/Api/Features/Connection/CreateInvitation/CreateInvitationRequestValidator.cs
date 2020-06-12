namespace BlazorHosted.Features.Connections
{
  using FluentValidation;
  
  public class CreateInvitationRequestValidator : AbstractValidator<CreateInvitationRequest>
  {

    public CreateInvitationRequestValidator()
    {
      //RuleFor
      //(
      //  aCreateInvitationRequest => aCreateInvitationRequest.
      //).NotEmpty().GreaterThan(0);
    }
  }
}