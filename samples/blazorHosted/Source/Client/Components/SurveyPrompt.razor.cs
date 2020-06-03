namespace BlazorHosted.Components
{
  using Microsoft.AspNetCore.Components;
  using BlazorHosted.Features.Bases;

  public partial class SurveyPrompt: BaseComponent
  {
    [Parameter]
    public string Title { get; set; } // Demonstrates how a parent component can supply parameters
  }
}
