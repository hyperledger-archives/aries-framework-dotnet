using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AgentFramework.Payments.NullPay
{
    public static class Configuration
    {
        public const string MethodName = "null";

        public static Task InitializeAsync()
        {
            return Task.Run(async () =>
            {
                await Task.Yield();
                nullpay_init();
            });
        }

#if __IOS__
        [DllImport("__Internal", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
#else
        [DllImport("nullpay", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
#endif
        internal static extern void nullpay_init();
    }
}
