using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Zal.Bridge.Tools
{
    internal static class Security
    {
        private static string password = "3sc3RLrpd17";
        private static Aes encryptor;
        private static Aes Encryptor {
            get { return encryptor ?? CreateAesEncryptor(); }
        }

        private static Aes CreateAesEncryptor()
        {
            encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(password));
            encryptor.IV = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            return encryptor;
        }

        public static string Encrypt(this string text)
        {
            return Encoding.UTF8.GetBytes(text).Encrypt();
        }

        public static string Encrypt(this byte[] bytes)
        {
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = Encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] encryptedBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(encryptedBytes, 0, encryptedBytes.Length);
        }

        public static byte[] Decrypt(this string encryptedText)
        {
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = Encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string text = String.Empty;
            byte[] bytes = Convert.FromBase64String(encryptedText);
            try
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                bytes = memoryStream.ToArray();
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }
            return bytes;
        }

        public static byte[] Zip(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var result = new MemoryStream())
            {
                using (var compressionStream = new GZipStream(result, CompressionMode.Compress))
                {
                    compressionStream.Write(bytes, 0, bytes.Length);
                }
                return result.ToArray();
            }
        }

        public static string Unzip(this byte[] bytes)
        {
            using (var compressed = new MemoryStream(bytes))
            using (var unzipped = new MemoryStream())
            {
                try
                {
                    using (var gZip = new GZipStream(compressed, CompressionMode.Decompress))
                    {
                        gZip.CopyTo(unzipped);
                    }
                    var str =  Encoding.UTF8.GetString(unzipped.ToArray());
                    return str;
                }catch(Exception e)
                {
                    return null;
                }
            }
        }
    }
}
