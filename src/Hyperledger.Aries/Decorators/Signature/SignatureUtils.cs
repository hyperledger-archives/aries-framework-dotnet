﻿using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Indy.CryptoApi;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Decorators.Signature
{
    /// <summary>
    /// Utility class for signing data for the usage in signature decorators.
    /// </summary>
    public static class SignatureUtils
    {
        /// <summary>
        /// Default signature type for signing data.
        /// </summary>
        public const string DefaultSignatureType =
            "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/signature/1.0/ed25519Sha512_single";

        /// <summary>
        /// Sign data supplied and return a signature decorator.
        /// </summary>
        /// <typeparam name="T">Data object type to sign.</typeparam>
        /// <param name="agentContext">Agent context.</param>
        /// <param name="data">Data to sign.</param>
        /// <param name="signerKey">Signers verkey.</param>
        /// <returns>Async signature decorator.</returns>
        public static async Task<SignatureDecorator> SignData<T>(IAgentContext agentContext, T data, string signerKey)
        {
            var dataJson = JsonConvert.SerializeObject(data);
            var epocData = new byte[8]; //TODO actually put the epoc representation in here

            var sigData = epocData.Concat(dataJson.GetUTF8Bytes()).ToArray();

            var sig = await Crypto.SignAsync(agentContext.Wallet, signerKey, sigData);

            var sigDecorator = new SignatureDecorator
            {
                SignatureType = DefaultSignatureType,
                SignatureData = sigData.ToBase64UrlString(),
                Signature = sig.ToBase64UrlString(),
                Signer = signerKey
            };

            return sigDecorator;
        }

        /// <summary>
        /// Unpack and verify signed data before casting it to the supplied type.
        /// </summary>
        /// <typeparam name="T">Type in which to cast the result to.</typeparam>
        /// <param name="decorator">Signature decorator to unpack and verify</param>
        /// <returns>Resulting data cast to type T.</returns>
        public static T UnpackAndVerifyData<T>(SignatureDecorator decorator)
        {
            var sigDataBytes = decorator.SignatureData.GetBytesFromBase64();
            var sigDataString = sigDataBytes.Skip(8).ToArray().GetUTF8String();
            return JsonConvert.DeserializeObject<T>(sigDataString);
        }
    }
}
