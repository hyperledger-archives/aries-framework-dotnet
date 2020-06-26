namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class SendRequestForProofRequest : BaseApiRequest, IRequest<SendRequestForProofResponse>
  {
    public const string RouteTemplate = "api/PresentProofs/SendRequestForProof";

    /// <summary>
    /// Set Properties and Update Docs
    /// </summary>
    /// <example>TODO</example>
    public string SampleProperty { get; set; }

    internal override string GetRoute() => $"{Route}?{nameof(SampleProperty)}={SampleProperty}&{nameof(CorrelationId)}={CorrelationId}";
  }
}