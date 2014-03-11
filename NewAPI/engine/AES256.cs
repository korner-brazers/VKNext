using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace NewAPI.engine
{
    class AES256
    {
        private SymmetricAlgorithm AESProvider = new AesCryptoServiceProvider();

        public AES256(string KeyAES)
        {
            AESProvider.KeySize = 256;
            AESProvider.Key = (new PasswordDeriveBytes(KeyAES, Encoding.ASCII.GetBytes(KeyAES), "SHA1", 1000)).GetBytes(256 / 8);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        public string Encrypt(string s)
        {
            AESProvider.GenerateIV();
            ICryptoTransform encryptor = AESProvider.CreateEncryptor();
            byte[] plainBytes = UnicodeEncoding.Unicode.GetBytes(s);
            byte[] secureBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            secureBytes = AESProvider.IV.Concat(secureBytes).ToArray();
            string result = Convert.ToBase64String(secureBytes);

            //Чистка ресурсов и возврат результатов
            s = null; secureBytes = null; plainBytes = null; encryptor.Dispose(); encryptor = null;
            AESProvider.Clear(); AESProvider.Dispose(); AESProvider = null;
            return result;
        }

        public string Decrypt(string s)
        {
            byte[] secureBytes = Convert.FromBase64String(s);
            AESProvider.IV = secureBytes.Take(16).ToArray();
            ICryptoTransform decryptor = AESProvider.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(secureBytes, 16, secureBytes.Length - 16);
            string result = UnicodeEncoding.Unicode.GetString(plainBytes);

            //Чистка ресурсов и возврат результатов
            s = null; plainBytes = null; decryptor.Dispose(); decryptor = null; secureBytes = null;
            AESProvider.Clear(); AESProvider.Dispose(); AESProvider = null;
            return result;
        }
    }
}
