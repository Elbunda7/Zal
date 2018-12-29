using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Domain.Tools
{
    public class Security
    {
        private string password = "3sc3RLrpd17";
        private byte[] key;
        private byte[] iv;
        Aes encryptor;

        public Security() {
            key = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(password));
            iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            CreateAesEncryptor();
        }

        private void CreateAesEncryptor() {
            encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
        }

        public string Encrypt(string text) {
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            cryptoStream.Write(textBytes, 0, textBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] encryptedBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string encryptedText = Convert.ToBase64String(encryptedBytes, 0, encryptedBytes.Length);
            return encryptedText;
        }

        public string Decrypt(string encryptedText) {
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string text = String.Empty;
            try {
                byte[] cipherBytes = Convert.FromBase64String(encryptedText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                text = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally {
                memoryStream.Close();
                cryptoStream.Close();
            }
            return text;
        }
    }
}
