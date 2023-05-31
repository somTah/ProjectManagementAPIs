using Microsoft.AspNetCore.Identity;
using ProjectManagementAPIs.Interfaces;
using System.Security.Cryptography;

namespace ProjectManagementAPIs.Repository
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            byte[] salt = GenerateSalt();
            byte[] hashedPassword = ComputeHash(password, salt);
            string passwordSalt = Convert.ToBase64String(salt);
            string passwordHash = Convert.ToBase64String(hashedPassword);

            return passwordSalt + ":" + passwordHash;
        }

        public bool Verify(string passwordHash, string password)
        {
            string[] parts = passwordHash.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);
            byte[] computedHash = ComputeHash(password, salt);

            return AreHashesEqual(storedHash, computedHash);
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private byte[] ComputeHash(string password, byte[] salt)
        {
            using (var hasher = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                return hasher.GetBytes(32);
            }
        }

        private bool AreHashesEqual(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
            {
                return false;
            }

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
