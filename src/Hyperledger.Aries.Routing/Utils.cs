using System.Security.Cryptography;
using System.Text;

namespace Hyperledger.Aries.Routing
{
    public static class Utils
    {
        /// <summary>
        /// Generate unique random alpha-numeric key
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static string GenerateRandomAsync(int maxSize)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[maxSize];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }
    }
}
