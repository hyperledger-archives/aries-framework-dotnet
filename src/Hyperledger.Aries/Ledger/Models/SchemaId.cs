namespace Hyperledger.Aries.Ledger.Models
{
    public struct SchemaId
    {
        public string Did { get; }

        public int TxnId => 2;
        
        public string Name { get; }
        
        public string Version { get; }

        public override string ToString()
        {
            return $"{Did}:{TxnId}:{Name}:{Version}";
        }

        public SchemaId(string schemaId)
        {
            var parts = schemaId.Split(':');
            Did = parts[0];
            Name = parts[2];
            Version = parts[3];
        }
    }
}
