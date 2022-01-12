using System.Linq;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Decorators.Service;
using Hyperledger.Aries.Utils;

namespace System
{
    /// <summary>
    /// Service decorator extensions
    /// </summary>
    public static class ServiceDecoratorExtensions
    {
        /// <summary>
        /// Get a service decorator representation for this provisioning record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="useDidKeyFormat"></param>
        /// <returns></returns>
        public static ServiceDecorator ToServiceDecorator(this ProvisioningRecord record, bool useDidKeyFormat = false)
        {
            return new ServiceDecorator
            {
                ServiceEndpoint = record.Endpoint.Uri,
                RoutingKeys = useDidKeyFormat ? record.Endpoint.Verkey.Select(DidUtils.ConvertVerkeyToDidKey) : record.Endpoint.Verkey,
                RecipientKeys = new [] { useDidKeyFormat ? DidUtils.ConvertVerkeyToDidKey(record.IssuerVerkey) : record.IssuerVerkey }
            };
        }
    }
}
