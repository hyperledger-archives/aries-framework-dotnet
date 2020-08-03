namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using MediatR;
  using System;

  public class CreateCredentialDefinitionRequest : BaseApiRequest, IRequest<CreateCredentialDefinitionResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "credential-definitions";

    /// <summary>
    /// Should these credentials be revocable
    /// </summary>
    public bool EnableRevocation { get; set; } = false;

    public bool RevocationRegistryAutoScale { get; set; }

    public Uri? RevocationRegistryBaseUri { get; set; }

    public int RevocationRegistrySize { get; set; }

    /// <summary>
    /// The Id of the Schema to be used for the Credential Definition
    /// </summary>
    /// <example>WgWxqztrNooG92RXvxSTWv:2:schema_name:1.0</example>
    public string SchemaId { get; set; } = null!;

    /// <summary>
    /// Credential definition identifier tag
    /// </summary>
    /// <example>default</example>
    public string? Tag { get; set; }

    internal override string GetRoute() => RouteTemplate;
  }
}
