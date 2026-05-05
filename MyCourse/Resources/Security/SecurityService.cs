using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace C971MobileAppDev.Resources.Security
{
    // Minimal, industry-appropriate local security helpers for storing/validating a PIN or secret.
    // Uses PBKDF2 (Rfc2898DeriveBytes) for hashing and SecureStorage to persist hashed value.
    public static class SecurityService
    {
        private const string StorageKey = "app_pin_hash_v1";
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public static string HashSecret(string secret)
        {
            if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(secret, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            var combined = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, combined, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, combined, SaltSize, HashSize);

            return Convert.ToBase64String(combined);
        }

        public static bool VerifySecret(string secret, string storedBase64)
        {
            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(storedBase64)) return false;

            var combined = Convert.FromBase64String(storedBase64);
            if (combined.Length != SaltSize + HashSize) return false;

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(combined, 0, salt, 0, SaltSize);
            var storedHash = new byte[HashSize];
            Buffer.BlockCopy(combined, SaltSize, storedHash, 0, HashSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(secret, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, storedHash);
        }

        public static async Task SaveHashedSecretAsync(string secret)
        {
            var hash = HashSecret(secret);
            await SecureStorage.Default.SetAsync(StorageKey, hash);
        }

        public static async Task<bool> VerifySavedSecretAsync(string secret)
        {
            try
            {
                var stored = await SecureStorage.Default.GetAsync(StorageKey);
                if (string.IsNullOrEmpty(stored)) return false;
                return VerifySecret(secret, stored);
            }
            catch
            {
                // SecureStorage may throw on some platforms; handle gracefully
                return false;
            }
        }

        public static void RemoveSavedSecret()
        {
            try
            {
                SecureStorage.Default.Remove(StorageKey);
            }
            catch
            {
                // Ignore removal errors
            }
        }
    }
}