using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Security
{
    public static class AesEncoder
    {
        private static byte[] key = { 30, 8, 143, 102, 38, 188, 120, 162, 106, 49, 202, 46, 91, 234, 120, 241, 41, 39, 31, 105, 8, 25, 148, 58, 146, 77, 140, 156, 65, 63, 59, 146 };

        public static bool Init()
        {
            string pid = SystemFunctions.GetHWID();
            key = pid.GetSHA256();
            return pid != "UnsafeEnvironment";
        }

        public static string ToAes256Base64(this string src)
        {
            return Convert.ToBase64String(src.ToAes256());
        }

        public static byte[] ToAes256(this string src)
        {
            Aes aes = Aes.Create();
            aes.GenerateIV();
            aes.Key = key;
            byte[] encrypted;
            ICryptoTransform crypt = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(src);
                    }
                }
                encrypted = ms.ToArray();
            }
            return encrypted.Concat(aes.IV).ToArray();
        }

        public static string FromAes256Base64(this string src)
        {
            try
            {
                return Convert.FromBase64String(src).FromAes256();
            }
            catch
            {
                return "";
            }
        }

        public static string FromAes256(this byte[] shifr)
        {
            byte[] bytesIv = new byte[16];
            byte[] mess = new byte[shifr.Length - 16];
            for (int i = shifr.Length - 16, j = 0; i < shifr.Length; i++, j++)
                bytesIv[j] = shifr[i];
            for (int i = 0; i < shifr.Length - 16; i++)
                mess[i] = shifr[i];

            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = bytesIv;

            byte[] data = mess;
            ICryptoTransform crypt = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
