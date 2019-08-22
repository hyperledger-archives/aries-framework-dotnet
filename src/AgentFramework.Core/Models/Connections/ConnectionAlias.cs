namespace AgentFramework.Core.Models.Connections
{
    /// <summary>
    /// Connection alias object for tagging 
    /// a connection record with an alias to
    /// give more context.
    /// </summary>
    public class ConnectionAlias
    {
        /// <summary>Initializes a new instance of the <see cref="ConnectionAlias"/> class.</summary>
        /// <param name="copy">The copy.</param>
        public ConnectionAlias(ConnectionAlias copy)
        {
            Name = copy.Name;
            ImageUrl = copy.ImageUrl;
        }

        /// <summary>Initializes a new instance of the <see cref="ConnectionAlias"/> class.</summary>
        public ConnectionAlias() { }

        /// <summary>
        /// Gets or sets the name of the alias for the connection.
        /// </summary>
        /// <value>
        /// The name of the alias for the connection.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the url of an image of the alias for the connection.
        /// </summary>
        /// <value>
        /// The image url of the alias for the connection.
        /// </value>
        public string ImageUrl { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Name={Name}, " +
            $"ImageUrl={ImageUrl}";
    }
}
