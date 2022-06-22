using System;
using System.Linq;
using System.Text.RegularExpressions;
using Multiformats.Base;

namespace Hyperledger.Aries.Utils
{
    /// <summary>
    /// Did utilities
    /// </summary>
    public static class DidUtils
    {
        private const string FULL_VERKEY_REGEX = @"^[1-9A-HJ-NP-Za-km-z]{43,44}$";
        private const string ABREVIATED_VERKEY_REGEX = @"^~[1-9A-HJ-NP-Za-km-z]{22}$";
        private const string DID_REGEX = @"^did:([a-z]+):([a-zA-z\d]+)";
        private const string DID_KEY_REGEX = @"^did:key:([1-9A-HJ-NP-Za-km-z]+)";
        private const string DIDKEY_PREFIX = "did:key";
        private const string BASE58_PREFIX = "z";
        private static readonly byte[] MULTICODEC_PREFIX_ED25519 = { 0xed, 0x01 };

        /// <summary>
        /// Sovrin DID method spec.
        /// </summary>
        public const string DidSovMethodSpec = "sov";

        /// <summary>
        /// Did Key method spec.
        /// </summary>
        public const string DidKeyMethodSpec = "key";

        /// <summary>
        /// Did Indy method spec.
        /// </summary>
        public const string DidIndyMethodSpec = "indy";

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
        /// Extracts the method specification from a DID.
        /// </summary>
        /// <param name="did">DID to extract the method spec from.</param>
        /// <returns></returns>
        public static string MethodSpecFromDid(string did)
        {
            var regExMatches = Regex.Matches(did, DID_REGEX);
            
            if (regExMatches.Count != 1 || regExMatches[0].Groups.Count < 3)
                return null;

            return regExMatches[0].Groups[1].Value;
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

        /// <summary>
        /// Check if a given string is a valid did:key representation
        /// </summary>
        /// <param name="didKey">Given string to check for did:key</param>
        /// <returns>Boolean indicating if the string is a valid did:key</returns>
        public static bool IsDidKey(string didKey)
        {
            if (didKey == null) 
                return false;
            
            return Regex.Matches(didKey, DID_KEY_REGEX).Count == 1;
        }
        
        /// <summary>
        /// Converts a base58 encoded ed25519 verkey into its did:key representation
        /// </summary>
        /// <param name="verkey">Base58 encoded string representation of a verkey</param>
        /// <returns>The did:key representation of a verkey as string</returns>
        public static string ConvertVerkeyToDidKey(string verkey)
        {
            if (IsFullVerkey(verkey) == false)
            {
                throw new ArgumentException($"Value {verkey} is no verkey", nameof(verkey));
            }

            var bytes = Multibase.Base58.Decode(verkey);
            bytes = MULTICODEC_PREFIX_ED25519.Concat(bytes).ToArray();
            string base58PublicKey = Multibase.Base58.Encode(bytes);

            return $"{DIDKEY_PREFIX}:{BASE58_PREFIX}{base58PublicKey}";
        }
        
        /// <summary>
        /// Converts a did:key of a ed25519 public key into a plain base58 representation 
        /// </summary>
        /// <param name="didKey">A did:key representation of a ed25519 as string</param>
        /// <returns>A plain base58 representation of that public key</returns>
        public static string ConvertDidKeyToVerkey(string didKey)
        {
            if (IsDidKey(didKey) == false)
            {
                throw new ArgumentException($"Value {didKey} is no did:key", nameof(didKey));
            }

            string base58EncodedKey = didKey.Substring($"{DIDKEY_PREFIX}:{BASE58_PREFIX}".Length);
            var bytes = Multibase.Base58.Decode(base58EncodedKey);
            var codec = bytes.Take(MULTICODEC_PREFIX_ED25519.Length).ToArray();
            if (codec.SequenceEqual(MULTICODEC_PREFIX_ED25519))
            {
                bytes = bytes.Skip(MULTICODEC_PREFIX_ED25519.Length).ToArray();
                return Multibase.Base58.Encode(bytes);
            }

            throw new ArgumentException($"Value {didKey} has missing ED25519 multicodec prefix", nameof(didKey));
        }

        /// <summary>
        /// Ensure a given string represents a supported DID method.
        /// Will transform unqualified verkeys into did:key format.
        /// </summary>
        /// <param name="didCandidate"></param>
        /// <returns></returns>
        /// <exception cref="AriesFrameworkException"></exception>
        public static string EnsureQualifiedDid(string didCandidate)
        {
            if (MethodSpecFromDid(didCandidate) == DidKeyMethodSpec ||
                MethodSpecFromDid(didCandidate) == DidSovMethodSpec)
            {
                return didCandidate;
            }

            if (IsVerkey(didCandidate))
            {
                return ConvertVerkeyToDidKey(didCandidate);
            }
            
            throw new AriesFrameworkException(ErrorCode.UnsupportedDidMethod);
        }
    }
}
