using System;
using Microsoft.Extensions.DependencyInjection;

namespace AgentFramework.Core.Configuration
{
    /// <summary>
    /// Agent Configuration Builder
    /// </summary>
    public class AgentFrameworkBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentFrameworkBuilder"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        internal AgentFrameworkBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Services collection 
        /// </summary>
        public IServiceCollection Services { get; }
    }
}
