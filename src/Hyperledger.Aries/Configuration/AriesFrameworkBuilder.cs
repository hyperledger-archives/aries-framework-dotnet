using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hyperledger.Aries.Configuration
{
    /// <summary>
    /// Agent Configuration Builder
    /// </summary>
    public class AriesFrameworkBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AriesFrameworkBuilder"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        internal AriesFrameworkBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Services collection 
        /// </summary>
        public IServiceCollection Services { get; }
    }
}
