namespace Hyperledger.Aries.AspNetCore.Configuration
{
  public class AgentSettings
  {
    public string AgentName { get; set; }
    public string EndpointUri { get; set; }
    public string GenesisFilename { get; set; }
    public string IssuerKeySeed { get; set; }
    public string PoolName { get; set; }
    public string WalletId { get; set; }
    public string WalletKey { get; set; }
  }
}
