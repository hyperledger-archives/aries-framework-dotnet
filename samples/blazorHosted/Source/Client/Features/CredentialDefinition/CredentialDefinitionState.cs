namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using BlazorState;
  using Hyperledger.Aries.Models.Records;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json.Serialization;

  internal partial class CredentialDefinitionState : State<CredentialDefinitionState>
  {
    private Dictionary<string, DefinitionRecord> _CredentialDefinitionRecords;

    [JsonIgnore]
    public IReadOnlyDictionary<string, DefinitionRecord> CredentialDefinitions => _CredentialDefinitionRecords;

    public IReadOnlyList<DefinitionRecord> CredentialDefinitionsAsList => _CredentialDefinitionRecords.Values.ToList();

    public CredentialDefinitionState() { }

    public override void Initialize()
    {
      _CredentialDefinitionRecords = new Dictionary<string, DefinitionRecord>();
    }
  }
}
