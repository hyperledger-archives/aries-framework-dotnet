namespace Hyperledger.Aries.OpenApi.Features.CredentialDefinitions
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Hyperledger.Aries.Models.Records;
  using System;

  public class GetCredentialDefinitionResponse : BaseResponse
  {
    public DefinitionRecord DefinitionRecord { get; set; } = null!;

    public GetCredentialDefinitionResponse() { }

    public GetCredentialDefinitionResponse(Guid aCorrelationId, DefinitionRecord aDefinitionRecord)
      : base(aCorrelationId)
    {
      DefinitionRecord = aDefinitionRecord;
    }
  }
}
