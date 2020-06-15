namespace BlazorHosted.Features.Bases
{
  using System;

  public abstract class BaseRequest
  {
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
