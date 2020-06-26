namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetCredentialsForProofRequest : BaseApiRequest, IRequest<GetCredentialsForProofResponse>
  {
    public const string RouteTemplate = "api/PresentProofs/GetCredentialsForProof";

    /// <summary>
    /// Set Properties and Update Docs
    /// </summary>
    /// <example>TODO</example>
    public string SampleProperty { get; set; }

    internal override string GetRoute() => $"{Route}?{nameof(SampleProperty)}={SampleProperty}&{nameof(CorrelationId)}={CorrelationId}";
  }
}