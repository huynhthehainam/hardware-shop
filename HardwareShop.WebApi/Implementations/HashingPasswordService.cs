using System.Security.Cryptography;
using HardwareShop.Application.Services;
using Microsoft.Extensions.Options;

namespace HardwareShop.WebApi.Implementations
{
    public class HashingConfiguration
    {
        public int SaltSize { get; set; } = 0;
        public int HashSize { get; set; } = 0;
        public string PrivateKey { get; set; } = string.Empty;

    }
    public class HashingPasswordService : IHashingPasswordService
    {
        private readonly HashingConfiguration hashingConfiguration;
        public HashingPasswordService(IOptions<HashingConfiguration> options)
        {
            this.hashingConfiguration = options.Value;
        }
        public string Hash(string text, int iterations)
        {
            Byte[] salt;
            RandomNumberGenerator.Create().GetBytes(salt = new Byte[hashingConfiguration.SaltSize]);
            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(text, salt, iterations);
            var hash = pbkdf2.GetBytes(hashingConfiguration.HashSize);
            // Combine salt and hash
            var hashBytes = new Byte[hashingConfiguration.SaltSize + hashingConfiguration.HashSize];
            Array.Copy(salt, 0, hashBytes, 0, hashingConfiguration.SaltSize);
            Array.Copy(hash, 0, hashBytes, hashingConfiguration.SaltSize, hashingConfiguration.HashSize);
            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);
            // Format hash with extra information
            return String.Format(hashingConfiguration.PrivateKey + "{0}${1}", iterations, base64Hash);

        }

        public string Hash(string text)
        {
            return Hash(text, 10000);
        }

        public bool IsHashedPasswordSupported(string text)
        {
            return text.Contains(hashingConfiguration.PrivateKey);
        }

        public bool Verify(string password, string hashedPassword)
        {
            // Check hash
            if (!IsHashedPasswordSupported(hashedPassword))
            {
                throw new NotSupportedException("The hash type is not supported");
            }
            // Extract iteration and Base64 String
            var splittedHashString = hashedPassword.Replace(hashingConfiguration.PrivateKey, "").Split('$');
            var iterations = Int32.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];
            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);
            // Get salt
            var salt = new Byte[hashingConfiguration.SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, hashingConfiguration.SaltSize);
            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            Byte[] hash = pbkdf2.GetBytes(hashingConfiguration.HashSize);
            // Get result
            for (var i = 0; i < hashingConfiguration.HashSize; i++)
            {
                if (hashBytes[i + hashingConfiguration.SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;

        }
    }
}
