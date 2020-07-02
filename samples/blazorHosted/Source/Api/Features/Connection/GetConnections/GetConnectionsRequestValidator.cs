namespace BlazorHosted.Features.Connections
{
  using FluentValidation;

  public class GetConnectionsRequestValidator : AbstractValidator<GetConnectionsRequest>
  {
    public GetConnectionsRequestValidator() { }
  }
}
