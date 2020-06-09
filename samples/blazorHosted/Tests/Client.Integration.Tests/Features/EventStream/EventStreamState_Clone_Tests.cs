namespace EventStreamState
{
  using AnyClone;
  using FluentAssertions;
  using System.Collections.Generic;
  using BlazorHosted.Features.EventStreams;
  using BlazorHosted.Client.Integration.Tests.Infrastructure;

  public class Clone_Should : BaseTest
  {
    private EventStreamState EventStreamState => Store.GetState<EventStreamState>();

    public Clone_Should(ClientHost aWebAssemblyHost) : base(aWebAssemblyHost) { }

    public void Clone()
    {
      //Arrange
      var events = new List<string> { "Event 1", "Event 2", "Event 3" };
      EventStreamState.Initialize(events);

      //Act
      var clone = EventStreamState.Clone() as EventStreamState;

      //Assert
      EventStreamState.Events.Count.Should().Be(clone.Events.Count);
      EventStreamState.Guid.Should().NotBe(clone.Guid);
      EventStreamState.Events[0].Should().Be(clone.Events[0]);
    }
  }
}
