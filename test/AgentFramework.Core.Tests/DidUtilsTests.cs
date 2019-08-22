using AgentFramework.Core.Utils;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class DidUtilsTests
    {
        private const string VALID_FULL_VERKEY = "MeHaPyPGsbBCgMKo13oWK7MeHaPyPGsbBCgMKo13oWK7";
        private const string VALID_ABBREVIATED_VERKEY = "~MeHaPyPGsbBCgMKo13oWK7";

        [Fact]
        public void CanDetectFullVerkey()
        {
            Assert.True(DidUtils.IsFullVerkey(VALID_FULL_VERKEY)); //Valid full verkey
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

    }
}