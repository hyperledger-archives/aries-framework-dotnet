namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Aries.Features.Handshakes.Connection.Models;
  using FluentValidation;

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