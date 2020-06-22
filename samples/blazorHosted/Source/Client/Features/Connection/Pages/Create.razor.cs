namespace BlazorHosted.Features.Connections.Pages
{
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Features.Connections;
  using Microsoft.AspNetCore.Components;
  using Microsoft.JSInterop;
  using System;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Connections.ConnectionState;

  public partial class Create : BaseComponent
  {
    public const string RouteTemplate = "/connections/create";

    [Inject] protected IJSRuntime JSRuntime { get; set; }

    public CreateInvitationRequest CreateInvitationRequest { get; set; }

    public static string GetRoute() => RouteTemplate;

    protected async Task ButtonClick()
    {
      _ = await Mediator.Send(new CreateConnectionAction());
    }

    public string DisplayInvitationUrl
    {
      get
      {
        string invitationUrl = ConnectionState?.InvitationUrl;
        return invitationUrl == null ? string.Empty : $"{invitationUrl.Substring(0, 30)}...";
      }
    }

    protected async Task CopyToClipboardAsync()
    {
      await JSRuntime.InvokeAsync<object>("navigator.clipboard.writeText", ConnectionState.InvitationUrl);
    }

    protected override async Task OnInitializedAsync()
    {
      Console.WriteLine("Where are you?");
      _ = await Mediator.Send(new CreateConnectionAction());

      await base.OnInitializedAsync();
    }
  }
}
