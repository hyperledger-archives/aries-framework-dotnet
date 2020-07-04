namespace Hyperledger.Aries.OpenApi.Features.Bases
{
  using System;

  public abstract class BaseRequest
  {
    public const string BaseUri = "aries/";

    /// <summary>
    /// Unique Identifier
    /// </summary>
    public Guid CorrelationId { get; set; }

    public BaseRequest()
    {
      CorrelationId = Guid.NewGuid();
    }
  }
}
