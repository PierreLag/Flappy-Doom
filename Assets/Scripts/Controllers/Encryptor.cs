using NUnit.Framework;
using System;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace FlappyDoom
{
    public class Encryptor
    {
        private readonly byte[] _key;

        public Encryptor(string key)
        {
            using var sha256 = SHA256.Create();

            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        public string Encrypt(string text)
        {
            using var aes = Aes.Create();
            aes.Key = _key;

            aes.GenerateIV();

            using var msEncrypt = new MemoryStream();

            msEncrypt.Write(aes.IV, 0, aes.IV.Length);

            using var encryptor = aes.CreateEncryptor();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            csEncrypt.Write(bytes, 0, bytes.Length);
            csEncrypt.FlushFinalBlock();

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            byte[] combinedBytes = Convert.FromBase64String(cipherText);

            if (combinedBytes.Length < 16)
            {
                throw new ArgumentException("Invalid cipher text length");
            }

            using var aes = Aes.Create();
            aes.Key = _key;

            byte[] iv = new byte[16];
            Array.Copy(combinedBytes, 0, iv, 0, 16);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();

            using var msDecrypt = new MemoryStream(combinedBytes, 16, combinedBytes.Length - 16);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    } 
}
