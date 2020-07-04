namespace CloneStateBehavior
{
  using FluentAssertions;
  using System;
  using System.Threading.Tasks;
  using Hyperledger.Aries.OpenApi.Features.Counters;
  using Hyperledger.Aries.OpenApi.Client.Integration.Tests.Infrastructure;
  using static Hyperledger.Aries.OpenApi.Features.Counters.CounterState;

  public class Should : BaseTest
  {
    private CounterState CounterState => Store.GetState<CounterState>();

    public Should(ClientHost aWebAssemblyHost) : base(aWebAssemblyHost) { }

    public async Task CloneState()
    {
      //Arrange
      CounterState.Initialize(aCount: 15);
      Guid preActionGuid = CounterState.Guid;

      // Create request
      var incrementCounterRequest = new IncrementCounterAction
      {
        Amount = -2
      };
      //Act
      await Send(incrementCounterRequest);

      //Assert
      CounterState.Guid.Should().NotBe(preActionGuid);
    }

    public void RollBackStateAndThrow_When_Exception()
    {
      // Arrange
      CounterState.Initialize(aCount: 22);
      Guid preActionGuid = CounterState.Guid;

      // Act
      var throwExceptionAction = new ThrowExceptionAction
      {
        Message = "Test Rollback of State"
      };

      // Act

      Func<Task> act = async () => await Send(throwExceptionAction);
      
      // Assert
      act.Should().Throw<Exception>().WithMessage(throwExceptionAction.Message);
      CounterState.Guid.Equals(preActionGuid);
    }
  }
}
