namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Models.Records;
  using System;
  using System.Collections.Generic;

  public class GetSchemasResponse : BaseResponse
  {
    public List<SchemaRecord> SchemaRecords { get; set; } = null!;

    public GetSchemasResponse() { }

    public GetSchemasResponse(Guid aCorrelationId, List<SchemaRecord> aSchemaRecords) : base(aCorrelationId)
    {
      SchemaRecords = aSchemaRecords;
    }
  }
}
