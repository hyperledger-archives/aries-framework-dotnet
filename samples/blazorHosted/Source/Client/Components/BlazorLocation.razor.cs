namespace Hyperledger.Aries.AspNetCore.Components
{
  using BlazorState.Services;
  using Microsoft.AspNetCore.Components;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public partial class BlazorLocation: BaseComponent
  {
    [Inject] public BlazorHostingLocation BlazorHostingLocation { get; set; }

    public string LocationName => BlazorHostingLocation.IsClientSide ? "Client Side" : "Server Side";
  }
}
