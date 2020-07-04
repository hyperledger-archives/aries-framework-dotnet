namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using FluentValidation;

  public class GetConnectionRequestValidator : AbstractValidator<GetConnectionRequest>
  {
    public GetConnectionRequestValidator()
    {
      RuleFor(aGetConnectionRequest => aGetConnectionRequest.ConnectionId)
        .NotEmpty();
    }
  }
}
