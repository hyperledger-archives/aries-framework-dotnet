namespace ApplicationState_
{
  using AnyClone;
  using FluentAssertions;
  using Hyperledger.Aries.AspNetCore.Features.Applications;
  using Hyperledger.Aries.AspNetCore.Client.Integration.Tests.Infrastructure;

  public class Clone_Should : BaseTest
  {
    private ApplicationState ApplicationState { get; set; }

    public Clone_Should(ClientHost aWebAssemblyHost) : base(aWebAssemblyHost)
    {
      ApplicationState = Store.GetState<ApplicationState>();
    }

    public void Clone()
    {
      //Arrange
      ApplicationState.Initialize(aName: "TestName", aLogo: "SomeUrl", aIsMenuExpanded: false);

      //Act
      ApplicationState clone = ApplicationState.Clone();

      //Assert
      ApplicationState.Should().NotBeSameAs(clone);
      ApplicationState.Name.Should().Be(clone.Name);
      ApplicationState.Logo.Should().Be(clone.Logo);
      ApplicationState.IsMenuExpanded.Should().Be(clone.IsMenuExpanded);
      ApplicationState.Guid.Should().NotBe(clone.Guid);
    }
  }
}
