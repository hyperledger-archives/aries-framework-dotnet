namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Schema record.
    /// </summary>
    public class SchemaRecord : RecordBase
    {
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        public override string TypeName => "AF.SchemaRecord";

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the version.</summary>
        /// <value>The version.</value>
        public string Version { get; set; }

        /// <summary>Gets or sets the attribute names.</summary>
        /// <value>The attribute names.</value>
        public string[] AttributeNames { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Name={Name}, " +
            $"Version={Version}, " +
            $"AttributeNames={string.Join(",", AttributeNames ?? new string[0])}, " +
            base.ToString();
    }
}