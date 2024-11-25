using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace SubscriptionAPI.Services
{
    public class IdempotencyService : IIdempotencyService
    {
        private readonly IMemoryCache _cache;
        private const string KeyPattern = @"^[A-Za-z0-9\-_]+$";

        public IdempotencyService(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Generate a unique Idempotency Key using alphanumeric characters, hyphens, and underscores.
        /// </summary>
        public string GenerateIdempotencyKey()
        {
            // Generate a random GUID as a byte array
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] byteArray = new byte[16]; // 16 bytes for 128-bit key
                rng.GetBytes(byteArray);

                // Convert the byte array to a base64 string, replacing + and / with - and _
                var base64Key = Convert.ToBase64String(byteArray)
                    .TrimEnd('=')  // Remove padding
                    .Replace('+', '-')  // Replace + with -
                    .Replace('/', '_');  // Replace / with _

                // Ensure the generated string fits the required pattern
                if (System.Text.RegularExpressions.Regex.IsMatch(base64Key, KeyPattern))
                {
                    return base64Key;
                }
                else
                {
                    throw new InvalidOperationException("Generated idempotency key does not match the required pattern.");
                }
            }
        }

        /// <summary>
        /// Check if the key has been used and if it's expired
        /// </summary>
        public bool IsKeyUsed(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        /// <summary>
        /// Store the key in cache with expiration time
        /// </summary>
        public void MarkKeyAsUsed(string key, TimeSpan expirationTime)
        {
            _cache.Set(key, true, expirationTime);
        }
    }
}
