using Hyperledger.Aries.Extensions;
using Newtonsoft.Json;
using System;

namespace Hyperledger.Aries.Ledger
{
    /// <summary>
    /// Transaction Author Agreement Acceptance Model
    /// </summary>
    public class IndyTaaAcceptance
    {
        /// <summary>
        /// The version of the agreement
        /// </summary>
        /// <value></value>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// The digest of the accepted text and version calculated as
        /// hex(sha256("version" + "text"))
        /// </summary>
        /// <value></value>
        [JsonProperty("digest")]
        public string Digest { get; set; }

        /// <summary>
        /// The acceptance timestap in Unix Epoch time format
        /// </summary>
        /// <value></value>
        [JsonProperty("acceptanceDate")]
        public ulong AcceptanceDate { get; set; }

        /// <summary>
        /// The acceptance mechanism from the AML
        /// </summary>
        [JsonProperty("mechanism")]
        public string Mechanism { get; set; }

        /// <summary>
        /// Determines if this <see cref="IndyTaaAcceptance"/> is for <paramref name="txnAuthorAgreement"/>.
        /// </summary>
        /// <param name="txnAuthorAgreement">Transaction Author Agreement</param>
        /// <returns>True if digest matches <paramref name="txnAuthorAgreement"/>.</returns>
        public bool IsAcceptanceOf(IndyTaa txnAuthorAgreement)
        {
            if (txnAuthorAgreement == null)
                return false;
            if (Digest == null)
                return false;

            string newDigest = GetDigest(txnAuthorAgreement);
            if (Digest.Equals(newDigest))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates a new instance of <see cref="IndyTaaAcceptance"/> using the <see cref="IndyTaa"/>. This generates the <see cref="IndyTaaAcceptance.Digest"/>.
        /// </summary>
        /// <param name="txnAuthorAgreement">Transaction Author Agreement</param>
        /// <param name="mechanism">The acceptance mechanism from the AML</param>
        /// <returns></returns>
        public static IndyTaaAcceptance CreateAcceptance(IndyTaa txnAuthorAgreement, string mechanism = "for_session")
        {
            return new IndyTaaAcceptance
            {
                Digest = GetDigest(txnAuthorAgreement),
                //Text = txnAuthorAgreement.Text, // cannot store TAA text in the wallet. Seeing: System.ArgumentException: 'Cannot marshal: Encountered unmappable character.'
                Version = txnAuthorAgreement.Version,
                AcceptanceDate = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Mechanism = mechanism
            };
        }

        private static string GetDigest(IndyTaa taa)
        {
            using (var shaAlgorithm = System.Security.Cryptography.SHA256.Create())
                return shaAlgorithm.ComputeHash(
                    $"{taa.Version}{taa.Text}"
                    .GetUTF8Bytes())
                .ToHexString();
        }
    }
}
