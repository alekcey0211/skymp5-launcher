using Force.Crc32;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UpdatesClient.Modules.SelfUpdater
{
    internal static class Hashing
    {
        public static string GetSHA512FromFile(FileStream file)
        {
            byte[] fileData = new byte[file.Length];
            file.Read(fileData, 0, (int)file.Length);

            return GetSHA512Hash(fileData);
        }
        public static string GetSHA512FromText(string text)
        {
            return GetSHA512Hash(Encoding.UTF8.GetBytes(text));
        }

        private static string GetSHA512Hash(byte[] source)
        {
            using (SHA512 algorithm = SHA512.Create())
            {
                byte[] hash = algorithm.ComputeHash(source);

                return new string(hash.SelectMany(a => a.ToString("X2")).ToArray());
            }
        }
        public static string GetMD5FromBytes(byte[] bytes)
        {
            return GetMD5Hash(bytes);
        }
        public static string GetMD5FromFile(FileStream file)
        {
            byte[] fileData = new byte[file.Length];
            file.Read(fileData, 0, (int)file.Length);

            return GetMD5Hash(fileData);
        }
        public static string GetMD5FromText(string text)
        {
            return GetMD5Hash(Encoding.UTF8.GetBytes(text));
        }
        private static string GetMD5Hash(byte[] source)
        {
            using (MD5 algorithm = MD5.Create())
            {
                byte[] hash = algorithm.ComputeHash(source);

                return new string(hash.SelectMany(a => a.ToString("X2")).ToArray());
            }
        }

        public static uint GetCRC32FromBytes(byte[] bytes)
        {
            return GetCRC32Hash(bytes);
        }
        private static uint GetCRC32Hash(byte[] source)
        {
            return Crc32Algorithm.Compute(source);
        }
    }
}
