namespace Hyperledger.Aries.AspNetCore.Configuration
{
  public class AriesOpenApiOptions
  {
    /// <summary>
    /// Enable Swagger UI
    /// </summary>
    /// <remarks>Default is true</remarks>
    public bool UseSwaggerUi { get; set; } = true;

    internal string RoutePrefix { get; set; }
    internal string SwaggerApiTitle { get; set; }
    internal string SwaggerEndPoint { get; set; }
  }
}
