using System;
using Hyperledger.Aries.Utils;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public class DidUtilsTests
    {
        private const string VALID_FULL_VERKEY = "MeHaPyPGsbBCgMKo13oWK7MeHaPyPGsbBCgMKo13oWK7";
        private const string ANOTHER_VALID_FULL_VERKEY = "XHhCzrFBTvrh2GsmHWRW4bpGYHdiPJbagSTFEMvFayc";
        private const string VALID_ABBREVIATED_VERKEY = "~MeHaPyPGsbBCgMKo13oWK7";
        private const string VALID_DID_KEY = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        private const string ORIG_VERKEY = "8HH5gYEeNc3z7PYXmd54d4x6qAfCNrqQqEB3nS7Zfu7K";
        private const string DERIVED_DID_KEY = "did:key:z6MkmjY8GnV5i9YTDtPETC2uUAW6ejw3nk5mXF5yci5ab7th";
        private const string VALID_SECP256K1_0 = "did:key:zQ3shokFTS3brHcDQrn82RUDfCZESWL1ZdCEJwekUDPQiYBme";

        [Fact]
        public void CanDetectFullVerkey()
        {
            Assert.True(DidUtils.IsFullVerkey(VALID_FULL_VERKEY)); //Valid full verkey
            Assert.True(DidUtils.IsFullVerkey(ANOTHER_VALID_FULL_VERKEY)); // Indy seems to generate verkeys with less than 256bit (only 43 chars)
            Assert.False(DidUtils.IsFullVerkey("")); 
            Assert.False(DidUtils.IsFullVerkey(VALID_ABBREVIATED_VERKEY)); //Valid abbreviated verkey
        }

        [Fact]
        public void CanDetectAbbreviatedVerkey()
        {
            Assert.True(DidUtils.IsAbbreviatedVerkey(VALID_ABBREVIATED_VERKEY));
            Assert.False(DidUtils.IsAbbreviatedVerkey(""));
            Assert.False(DidUtils.IsAbbreviatedVerkey(VALID_FULL_VERKEY));
        }

        [Fact]
        public void CanDetectVerkey()
        {
            Assert.True(DidUtils.IsVerkey(VALID_FULL_VERKEY));
            Assert.True(DidUtils.IsVerkey(VALID_ABBREVIATED_VERKEY));
            Assert.False(DidUtils.IsVerkey(""));
        }

        [Fact]
        public void CanDetectDidKey()
        {
            Assert.True(DidUtils.IsDidKey(VALID_DID_KEY));
            Assert.True(DidUtils.IsDidKey(VALID_SECP256K1_0));
            Assert.False(DidUtils.IsDidKey(VALID_FULL_VERKEY));
            Assert.False(DidUtils.IsDidKey(""));
        }
        
        [Fact]
        public void CanConvertDidKeyToVerkey()
        {
            var result = DidUtils.ConvertDidKeyToVerkey(DERIVED_DID_KEY);
            
            Assert.Equal(ORIG_VERKEY, result);
        }
        
        [Fact]
        public void CanConvertVerkeyToDidKey()
        {
            var result = DidUtils.ConvertVerkeyToDidKey(ORIG_VERKEY);
            
            Assert.Equal(DERIVED_DID_KEY, result);
        }

        [Fact]
        public void ConvertNonEd25519KeysToDidKeyWillThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DidUtils.ConvertDidKeyToVerkey(VALID_SECP256K1_0));
        }
    }
}