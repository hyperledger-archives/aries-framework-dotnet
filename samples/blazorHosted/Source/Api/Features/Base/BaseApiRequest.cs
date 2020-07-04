namespace Hyperledger.Aries.OpenApi.Features.Bases
{
  public abstract class BaseApiRequest : BaseRequest
  {
    internal abstract string GetRoute();
  }
}
