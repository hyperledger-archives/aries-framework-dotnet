using System;
using System.Text;
using System.Threading.Tasks;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.CryptoApi;
using Hyperledger.Indy.WalletApi;
using Multiformats.Base;

namespace Hyperledger.Aries.Decorators.Attachments
{
    public static class AttachmentContentExtensions
    {
        /// <summary>
        /// Sign attachment content using json web signature
        /// </summary>
        /// <param name="content">The attachment content to be signed.</param>
        /// <param name="wallet">The wallet.</param>
        /// <param name="verkey">The verkey to be used for the signing.</param>
        /// <exception cref="NullReferenceException">Throws if payload is null.</exception>
        public static async Task SignWithJsonWebSignature(this AttachmentContent content, Wallet wallet, string verkey)
        {
            var payload = content.Base64;
            if (payload == null) throw new NullReferenceException("No data to sign");
            
            var did = DidUtils.ConvertVerkeyToDidKey(verkey);

            var protectedHeader = new
            {
                alg = "EdDSA",
                kid = did,
                jwk = new
                {
                    kty = "OKP",
                    crv = "Ed25519",
                    x = Multibase.Base58.Decode(verkey).ToBase64UrlString(),
                    kid = did
                }
            }.ToJson().ToBase64Url();

            var message = $"{protectedHeader}.{payload}";

            var signature = (await Crypto.SignAsync(wallet, verkey, Encoding.ASCII.GetBytes(message))).ToBase64UrlString();

            content.JsonWebSignature = new JsonWebSignature
            {
                Header = new JsonWebSignatureHeader {Kid = did},
                Protected = protectedHeader,
                Signature = signature
            };
        }

        /// <summary>
        /// Verify the json web signature of an attachment
        /// </summary>
        /// <param name="content">The attachment content to be verified.</param>
        /// <returns>True - signature is valid; False - signature is missing or invalid.</returns>
        public static async Task<bool> VerifyJsonWebSignature(this AttachmentContent content)
        {
            try
            {
                var did = content.JsonWebSignature.Header.Kid;
                
                var verkey = DidUtils.ConvertDidKeyToVerkey(did);
                
                var message = $"{content.JsonWebSignature.Protected}.{content.Base64}";
            
                return await Crypto.VerifyAsync(verkey, Encoding.ASCII.GetBytes(message),
                    content.JsonWebSignature.Signature.GetBytesFromBase64());
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
