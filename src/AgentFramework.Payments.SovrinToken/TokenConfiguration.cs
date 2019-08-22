using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AgentFramework.Payments.SovrinToken
{
    public static class TokenConfiguration
    {
        public const string MethodName = "sov";

        public static Task InitializeAsync()
        {
            return Task.Run(async () =>
            {
                await Task.Yield();
                sovtoken_init();
            });
        }

#if __IOS__
        [DllImport("__Internal", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
#else
        [DllImport("sovtoken", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
#endif
        internal static extern void sovtoken_init();
    }
}
