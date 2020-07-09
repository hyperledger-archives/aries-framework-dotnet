namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Models.Records;

  public class GetCredentialDefinitionsResponse : BaseResponse
  {
    public List<DefinitionRecord> DefinitionRecords { get; set; } = null!;
    public GetCredentialDefinitionsResponse() { }

    public GetCredentialDefinitionsResponse(Guid aCorrelationId, List<DefinitionRecord> aDefinitionRecords)
      : base(aCorrelationId) 
    {
      DefinitionRecords = aDefinitionRecords;
    }
  }
}
