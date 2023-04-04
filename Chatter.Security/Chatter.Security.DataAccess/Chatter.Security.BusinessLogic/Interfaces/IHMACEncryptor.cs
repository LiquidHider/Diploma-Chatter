using Microsoft.Extensions.Configuration;

namespace Chatter.Security.Core.Interfaces
{
    public interface IHMACEncryptor
    {
        string CreateRandomPasswordKey(IConfiguration config);

        string EncryptPassword(string password, byte[] key);

        bool Verify(string decryptedValue, string encryptedValue, byte[] key);
    }
}
