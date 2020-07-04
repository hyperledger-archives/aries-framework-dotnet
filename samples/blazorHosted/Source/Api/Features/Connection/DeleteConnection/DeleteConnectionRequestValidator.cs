namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using FluentValidation;

  public class DeleteConnectionRequestValidator : AbstractValidator<DeleteConnectionRequest>
  {
    public DeleteConnectionRequestValidator()
    {
      RuleFor(aDeleteConnectionRequest => aDeleteConnectionRequest.ConnectionId)
        .NotEmpty();
    }
  }
}
