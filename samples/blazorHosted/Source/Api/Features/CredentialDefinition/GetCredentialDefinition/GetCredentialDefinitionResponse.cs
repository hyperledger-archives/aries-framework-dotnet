namespace BlazorHosted.Features.CredentialDefinitions
{
  using BlazorHosted.Features.Bases;
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
