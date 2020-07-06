namespace Hyperledger.Aries.AspNetCore.Components
{
  using Jdenticon;
  using Microsoft.AspNetCore.Components;
  using System;
  using System.Threading.Tasks;

  public partial class Identicon : ComponentBase
  {
    [Parameter] public string Value { get; set; }
    [Parameter] public int Size { get; set; }
    private MarkupString SvgMarkupString { get; set; }

    private string CreateSvg()
    {
      var identicon = Jdenticon.Identicon.FromValue(Value, Size);
      return identicon.ToSvg();
    }

    protected override Task OnParametersSetAsync()
    {
      SvgMarkupString = new MarkupString(CreateSvg());
      return base.OnParametersSetAsync();
    }
  }
}
