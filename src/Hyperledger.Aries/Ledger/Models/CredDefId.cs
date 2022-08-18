using System;

namespace Hyperledger.Aries.Ledger.Models
{
    /// <summary>
    /// Credential Definition Id used to resolve a credential definition from an indy ledger
    /// </summary>
    public readonly struct CredDefId
    {
        /// <summary>
        /// Issuer's Did
        /// </summary>
        public string Did { get; }

        /// <summary>
        /// Transaction Type Id
        /// </summary>
        public int TxnId => 3;

        /// <summary>
        /// Signature Type - currently supports only "CL"
        /// </summary>
        public string SignatureType => "CL";
        
        /// <summary>
        /// Sequence number of the credential definition within the domain ledger
        /// </summary>
        public int SeqNo { get; }
        
        /// <summary>
        /// Chosen tag for this credential definition
        /// </summary>
        public string Tag { get; }
        
        /// <summary>
        /// Create a <see cref="CredDefId"/> object from an id string.
        /// </summary>
        /// <param name="credDefId">The credential definition id as string.</param>
        /// <exception cref="ArgumentNullException">Throws if string is null or empty.</exception>
        public CredDefId(string credDefId)
        {
            if(string.IsNullOrEmpty(credDefId)) throw new ArgumentNullException(nameof(credDefId));
            
            var parts = credDefId.Split(':');
            Did = parts[0];
            SeqNo = int.Parse(parts[3]);
            Tag = parts[4];
        }
        
        /// <summary>
        /// The credential definition id.
        /// </summary>
        public override string ToString()
        {
            return $"{Did}:{TxnId}:{SignatureType}:{SeqNo}:{Tag}";
        }
    }
}
