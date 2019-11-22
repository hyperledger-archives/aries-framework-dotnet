using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Payments.SovrinToken
{
    /// <summary>
    /// Token Configuration
    /// </summary>
    public static class TokenConfiguration
    {
        /// <summary>
        /// Method Name
        /// </summary>
        public const string MethodName = "sov";

        /// <summary>
        /// Initializes the internal static library
        /// </summary>
        /// <returns></returns>
        public static Task InitializeAsync() => Task.Run(async () =>
        {
            await Task.Yield();
            sovtoken_init();
        });

#if __IOS__
        [DllImport("__Internal", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
#else
        [DllImport("sovtoken", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
#endif
        internal static extern void sovtoken_init();
    }
}
