namespace Hyperledger.Aries.Features.ProblemReport
{
    /// <summary>
    /// Problem impact severity.
    /// </summary>
    public enum ProblemImpact
    {
        /// <summary>
        /// Message level impact.
        /// </summary>
        message,
        /// <summary>
        /// Thread level impact.
        /// </summary>
        thread,
        /// <summary>
        /// Connection level impact.
        /// </summary>
        connection
    }
}
