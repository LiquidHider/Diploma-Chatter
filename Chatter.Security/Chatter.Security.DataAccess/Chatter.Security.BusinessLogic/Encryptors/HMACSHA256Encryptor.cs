using Chatter.Security.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Chatter.Security.Core.Encryptors
{
    public class HMACSHA256Encryptor : IHMACEncryptor
    {
        public string CreateRandomPasswordKey(IConfiguration config)
        {
            var chars = config["EncryptionKeyChars"];

            var stringChars = new char[32];

            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            string resultString = new string(stringChars);

            return resultString;
        }

        public string EncryptPassword(string password, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashValue = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashValue);
            }
        }
    }
}
