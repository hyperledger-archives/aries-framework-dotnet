namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using BlazorState;
  using Hyperledger.Aries.Features.IssueCredential;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json.Serialization;

  internal partial class CredentialState : State<CredentialState>
  {
    private Dictionary<string, CredentialRecord> _CredentialRecords { get; set; } = null!;

    [JsonIgnore]
    public IReadOnlyDictionary<string, CredentialRecord> Credentials => _CredentialRecords;

    public IReadOnlyList<CredentialRecord> CredentialsAsList => _CredentialRecords.Values.ToList();
    public CredentialState()
    {
      _CredentialRecords = new Dictionary<string, CredentialRecord>();
    }

    public override void Initialize()
    {
      _CredentialRecords.Clear();
    }

  }
}
