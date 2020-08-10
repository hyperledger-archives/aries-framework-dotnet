namespace CounterState
{
  using AnyClone;
  using FluentAssertions;
  using Hyperledger.Aries.AspNetCore.Features.Counters;
  using Hyperledger.Aries.AspNetCore.Client.Integration.Tests.Infrastructure;

  public class Clone_Should : BaseTest
  {
    private CounterState CounterState { get; set; }

    public Clone_Should(ClientHost aWebAssemblyHost) : base(aWebAssemblyHost)
    {
      CounterState = Store.GetState<CounterState>();
    }

    public void Clone()
    {
      //Arrange
      CounterState.Initialize(aCount: 15);

      //Act
      var clone = CounterState.Clone() as CounterState;

      //Assert
      CounterState.Should().NotBeSameAs(clone);
      CounterState.Count.Should().Be(clone.Count);
      CounterState.Guid.Should().NotBe(clone.Guid);
    }
  }
}
