using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Security
{
    public static class SHA256Hashing
    {
        public static byte[] GetSHA256(this string str)
        {
            return Encoding.UTF8.GetBytes(str).GetSHA256();
        }
        public static string GetSHA256String(this string str)
        {
            byte[] hash = str.GetSHA256();
            return new string(hash.SelectMany(a => a.ToString("X2")).ToArray());
        }

        public static byte[] GetSHA256(this byte[] source)
        {
            using (SHA256 algorithm = SHA256.Create())
            {
                return algorithm.ComputeHash(source);
            }
        }

    }
}
