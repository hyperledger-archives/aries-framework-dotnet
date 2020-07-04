namespace Hyperledger.Aries.OpenApi.Features.Schemas
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Hyperledger.Aries.Models.Records;
  using System;

  public class GetSchemaResponse : BaseResponse
  {
    public SchemaRecord? SchemaRecord { get; set; }

    public GetSchemaResponse() { }

    public GetSchemaResponse(Guid aCorrelationId, SchemaRecord aSchemaRecord) : base(aCorrelationId)
    {
      SchemaRecord = aSchemaRecord;
    }
  }
}
