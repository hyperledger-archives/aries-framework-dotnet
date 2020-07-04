namespace Hyperledger.Aries.OpenApi.Features.Credentials
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class RemoveCredentialResponse : BaseResponse
  {
    public RemoveCredentialResponse() { }

    public RemoveCredentialResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
