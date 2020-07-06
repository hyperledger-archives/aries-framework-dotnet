namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using MediatR;
  using System.Collections.Generic;

  public class CreateSchemaRequest : BaseApiRequest, IRequest<CreateSchemaResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "schemas";

    /// <summary>
    /// The list of Attribute Names to be incldude in the Schema.
    /// </summary>
    public List<string> AttributeNames { get; set; } = null!;

    /// <summary>
    /// The Name of the Schema to create.
    /// </summary>
    /// <example>EducationDegree</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The Version of the Schema to create.
    /// </summary>
    /// <example>EducationDegree</example>
    public string Version { get; set; } = null!;

    public CreateSchemaRequest() { }

    public CreateSchemaRequest(string aName, string aVersion, List<string> aAttributeNames)
    {
      Name = aName;
      Version = aVersion;
      AttributeNames = aAttributeNames;
    }

    internal override string GetRoute() => RouteTemplate;
  }
}
