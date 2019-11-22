namespace Hyperledger.Aries.Features.ProblemReport
{
    /// <summary>
    /// Enumeration of possible retry parties.
    /// </summary>
    public enum RetryParty
    {
        /// <summary>
        /// You as the retry party.
        /// </summary>
        you,
        /// <summary>
        /// Me as the retry party.
        /// </summary>
        me,
        /// <summary>
        /// Both parties retry.
        /// </summary>
        both,
        /// <summary>
        /// No party retries.
        /// </summary>
        none
    }
}
