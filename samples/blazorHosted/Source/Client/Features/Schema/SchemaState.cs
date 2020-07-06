namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using BlazorState;
  using Hyperledger.Aries.Models.Records;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json.Serialization;

  internal partial class SchemaState : State<SchemaState>
  {
    private Dictionary<string, SchemaRecord> _SchemaRecords;

    [JsonIgnore]
    public IReadOnlyDictionary<string, SchemaRecord> Schemas => _SchemaRecords;

    public IReadOnlyList<SchemaRecord> SchemasAsList => _SchemaRecords.Values.ToList();

    public SchemaState() { }

    public override void Initialize()
    {
      _SchemaRecords = new Dictionary<string, SchemaRecord>();
    }
  }
}
