namespace Hyperledger.Aries.Configuration
{
  using System.Collections.Generic;
  using System.Reflection;
  public class AriesOpenApiOptions
  {
    ///// <summary>
    ///// Assemblies to be searched for MediatR Requests
    ///// </summary>
    public IEnumerable<Assembly> Assemblies { get; set; }

    public bool UseSwaggerUI { get; set; } = true;

    public AriesOpenApiOptions()
    {
      Assemblies = new Assembly[] { };
    }
  }
}

