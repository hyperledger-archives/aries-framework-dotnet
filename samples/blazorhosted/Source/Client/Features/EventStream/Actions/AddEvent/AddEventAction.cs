namespace blazorhosted.Features.EventStreams
{
  using blazorhosted.Features.Bases;

  internal partial class EventStreamState
  {
    public class AddEventAction : BaseAction
    {
      public string Message { get; set; }
    }
  }
}
