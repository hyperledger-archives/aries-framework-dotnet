namespace Hyperledger.Aries.OpenApi.Features.Schemas
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using System;

  public class CreateSchemaResponse : BaseResponse
  {
    public string SchemaId { get; set; } = null!;

    public CreateSchemaResponse() { }

    public CreateSchemaResponse(Guid aCorrelationId, string aSchemaId) : base(aCorrelationId)
    {
      SchemaId = aSchemaId;
    }
  }
}
