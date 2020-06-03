namespace BlazorHosted.Features.EventStreams
{
  using BlazorHosted.Features.Bases;

  internal partial class EventStreamState
  {
    public class AddEventAction : BaseAction
    {
      public string Message { get; set; }
    }
  }
}
