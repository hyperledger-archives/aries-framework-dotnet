namespace BlazorHosted.Features.Connections
{
  using FluentValidation;
  using Hyperledger.Aries.Features.DidExchange;

  public class CreateInvitationRequestValidator : AbstractValidator<CreateInvitationRequest>
  {

    public CreateInvitationRequestValidator()
    {
      RuleFor(aCreateInvitationRequest => aCreateInvitationRequest.InviteConfiguration).NotNull().SetValidator(new InviteConfigurationValidator());
    }

    class InviteConfigurationValidator : AbstractValidator<InviteConfiguration>
    {

      public InviteConfigurationValidator()
      {
        // We currently only support AutoAcceptConnections 
        RuleFor(aInviteConfiguration => aInviteConfiguration.AutoAcceptConnection)
          .Must(aAutoAcceptConnection => aAutoAcceptConnection == true);
      }
    }
  }
}