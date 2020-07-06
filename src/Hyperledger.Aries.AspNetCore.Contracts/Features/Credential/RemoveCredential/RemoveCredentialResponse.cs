namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class RemoveCredentialResponse : BaseResponse
  {
    public RemoveCredentialResponse() { }

    public RemoveCredentialResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
