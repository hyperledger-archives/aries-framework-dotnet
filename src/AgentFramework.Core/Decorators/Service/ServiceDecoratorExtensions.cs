using AgentFramework.Core.Decorators.Service;
using AgentFramework.Core.Models.Records;

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
        /// <returns></returns>
        public static ServiceDecorator ToServiceDecorator(this ProvisioningRecord record)
        {
            return new ServiceDecorator
            {
                ServiceEndpoint = record.Endpoint.Uri,
                RoutingKeys = new [] { record.Endpoint.Verkey },
                RecipientKeys = new [] { record.IssuerVerkey }
            };
        }
    }
}