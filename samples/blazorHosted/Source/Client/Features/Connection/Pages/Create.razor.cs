namespace Hyperledger.Aries.AspNetCore.Features.Connections.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Microsoft.AspNetCore.Components;
  using Microsoft.JSInterop;
  using System.Threading.Tasks;
  using static Hyperledger.Aries.AspNetCore.Features.Connections.ConnectionState;
  using static Hyperledger.Aries.AspNetCore.Features.Wallets.WalletState;

  public partial class Create : BaseComponent
  {
    public const string RouteTemplate = "/connections/create";

    public string DisplayInvitationUrl
    {
      get
      {
        string invitationUrl = ConnectionState?.InvitationUrl;
        return invitationUrl == null ? string.Empty : $"{invitationUrl.Substring(0, 30)}...";
      }
    }

    [Inject] protected IJSRuntime JSRuntime { get; set; }

    public static string GetRoute() => RouteTemplate;

    protected async Task ButtonClick()
    {
      _ = await Mediator.Send(new CreateConnectionAction());
    }

    protected async Task CopyToClipboardAsync()
    {
      await JSRuntime.InvokeAsync<object>("navigator.clipboard.writeText", ConnectionState.InvitationUrl);
    }

    protected override async Task OnInitializedAsync()
    {
      _ = await Mediator.Send(new FetchWalletAction());
      _ = await Mediator.Send(new CreateConnectionAction());

      await base.OnInitializedAsync();
    }  
  }
}
