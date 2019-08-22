using System.Text.RegularExpressions;

namespace AgentFramework.Core.Utils
{
    /// <summary>
    /// Did utilities
    /// </summary>
    public static class DidUtils
    {
        private const string FULL_VERKEY_REGEX = @"^[1-9A-HJ-NP-Za-km-z]{44}$";
        private const string ABREVIATED_VERKEY_REGEX = @"^~[1-9A-HJ-NP-Za-km-z]{22}$";
        private const string DID_REGEX = @"^did:([a-z]+):([a-zA-z\d]+)";

        /// <summary>
        /// Sovrin DID method spec.
        /// </summary>
        public const string DidSovMethodSpec = "sov";

        /// <summary>
        /// Constructs a DID from a method spec and identifier.
        /// </summary>
        /// <param name="methodSpec">DID method spec.</param>
        /// <param name="identifier">Identifier to use in DID.</param>
        /// <returns>DID.</returns>
        public static string ToDid(string methodSpec, string identifier) => $"did:{methodSpec}:{identifier}";

        /// <summary>
        /// Extracts the identifier from a DID.
        /// </summary>
        /// <param name="did">DID to extract the identifier from.</param>
        /// <returns>Identifier.</returns>
        public static string IdentifierFromDid(string did)
        {
            var regExMatches = Regex.Matches(did, DID_REGEX);

            if (regExMatches.Count != 1 || regExMatches[0].Groups.Count != 3)
                return null;
            
            return regExMatches[0].Groups[2].Value;
        }

        /// <summary>
        /// Check a base58 encoded string against a regex expression
        /// to determine if it is a full valid verkey
        /// </summary>
        /// <param name="verkey">Base58 encoded string representation of a verkey</param>
        /// <returns>Boolean indicating if the string is a valid verkey</returns>
        public static bool IsFullVerkey(string verkey)
        {
            return Regex.Matches(verkey, FULL_VERKEY_REGEX).Count == 1;
        }

        /// <summary>
        /// Check a base58 encoded string against a regex expression
        /// to determine if it is a abbreviated valid verkey
        /// </summary>
        /// <param name="verkey">Base58 encoded string representation of a abbreviated verkey</param>
        /// <returns>Boolean indicating if the string is a valid abbreviated verkey</returns>
        public static bool IsAbbreviatedVerkey(string verkey)
        {
            return Regex.Matches(verkey, ABREVIATED_VERKEY_REGEX).Count == 1;
        }

        /// <summary>
        /// Check a base58 encoded string to determine 
        /// if it is a valid verkey
        /// </summary>
        /// <param name="verkey">Base58 encoded string representation of a verkey</param>
        /// <returns>Boolean indicating if the string is a valid verkey</returns>

        public static bool IsVerkey(string verkey)
        {
            return IsAbbreviatedVerkey(verkey)|| IsFullVerkey(verkey);
        }
    }
}
