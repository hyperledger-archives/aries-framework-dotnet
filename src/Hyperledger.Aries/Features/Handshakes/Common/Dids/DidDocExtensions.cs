using System.Collections.Generic;
using Hyperledger.Aries.Configuration;

namespace Hyperledger.Aries.Features.Handshakes.Common.Dids
{
    /// <summary>
    /// Extensions for interacting with DID docs.
    /// </summary>
    public static class DidDocExtensions
    {
        /// <summary>
        /// Default key type.
        /// </summary>
        public const string DefaultKeyType = "Ed25519VerificationKey2018";

        /// <summary>
        /// Constructs my DID doc in a pairwise relationship from a connection record and the agents provisioning record.
        /// </summary>
        /// <param name="connection">Connection record.</param>
        /// <param name="provisioningRecord">Provisioning record.</param>
        /// <returns>DID Doc</returns>
        public static DidDoc MyDidDoc(this ConnectionRecord connection, ProvisioningRecord provisioningRecord)
        {
            var doc = new DidDoc
            {
                Id = connection.MyDid,
                Keys = new List<DidDocKey>
                {
                    new DidDocKey
                    {
                        Id = $"{connection.MyDid}#keys-1",
                        Type = DefaultKeyType,
                        Controller = connection.MyDid,
                        PublicKeyBase58 = connection.MyVk
                    }
                }
            };

            if (!string.IsNullOrEmpty(provisioningRecord.Endpoint.Uri))
            {
                doc.Services = new List<IDidDocServiceEndpoint>
                {
                    new IndyAgentDidDocService
                    {
                        Id = $"{connection.MyDid};indy",
                        ServiceEndpoint = provisioningRecord.Endpoint.Uri,
                        RecipientKeys = connection.MyVk != null ? new[] { connection.MyVk } : new string[0],
                        RoutingKeys = provisioningRecord.Endpoint?.Verkey != null ? provisioningRecord.Endpoint.Verkey : new string[0]
                    }
                };
            }

            return doc;
        }

        /// <summary>
        /// Constructs their DID doc in a pairwise relationship from a connection record.
        /// </summary>
        /// <param name="connection">Connectio record.</param>
        /// <returns>DID Doc</returns>
        public static DidDoc TheirDidDoc(this ConnectionRecord connection)
        {
            return new DidDoc
            {
                Id = connection.TheirDid,
                Keys = new List<DidDocKey>
                {
                    new DidDocKey
                    {
                        Id = $"{connection.TheirDid}#keys-1",
                        Type = DefaultKeyType,
                        Controller = connection.TheirDid,
                        PublicKeyBase58 = connection.TheirVk
                    }
                },
                Services = new List<IDidDocServiceEndpoint>
                {
                    new IndyAgentDidDocService
                    {
                        Id = $"{connection.TheirDid};indy",
                        ServiceEndpoint = connection.Endpoint.Uri,
                        RecipientKeys = connection.TheirVk != null ? new[] { connection.TheirVk } : new string[0],
                        RoutingKeys = connection.Endpoint?.Verkey != null ? connection.Endpoint.Verkey : new string[0]
                    }
                }
            };
        }
    }
}
