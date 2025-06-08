using BCrypt.Net;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Infrastructure.Services
{
    /// <summary>
    /// Password Hashing Service Implementation - the security vault for user passwords.
    /// Think of this as the high-tech security system that transforms passwords
    /// into unreadable codes that can't be reverse-engineered.
    /// </summary>
    public class PasswordHashingService : IPasswordHashingService
    {
        private const int WorkFactor = 12; // BCrypt work factor - higher is more secure but slower

        /// <summary>
        /// Hash a plain text password securely.
        /// Like putting a password through a one-way encryption machine
        /// that scrambles it beyond recognition.
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // Generate a salt and hash the password
            // BCrypt automatically handles salt generation and incorporates it into the hash
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Verify a plain text password against a stored hash.
        /// Like checking if the key matches the complex lock pattern
        /// without having to reverse-engineer the lock.
        /// </summary>
        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                // BCrypt.Verify handles extracting the salt from the hash
                // and comparing it with the provided password
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (Exception)
            {
                // If verification fails for any reason (corrupted hash, etc.), return false
                return false;
            }
        }

        /// <summary>
        /// Check if a password hash needs to be rehashed (upgraded).
        /// Like checking if an old security system needs updating.
        /// </summary>
        public bool NeedsRehashing(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return true;

            try
            {
                // Check if the hash was created with a different work factor
                var hashInfo = BCrypt.Net.BCrypt.PasswordUtil.GetPasswordInfo(hash);
                return hashInfo.WorkFactor < WorkFactor;
            }
            catch (Exception)
            {
                // If we can't parse the hash, it probably needs rehashing
                return true;
            }
        }

        /// <summary>
        /// Generate a secure random password.
        /// Like having the security system create a new, complex access code.
        /// </summary>
        public string GeneratePassword(int length = 12, bool includeSpecialChars = true)
        {
            if (length < 8)
                throw new ArgumentException("Password length must be at least 8 characters", nameof(length));

            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var charSet = lowercase + uppercase + numbers;
            if (includeSpecialChars)
                charSet += specialChars;

            var random = new Random();
            var password = new char[length];

            // Ensure at least one character from each required set
            password[0] = lowercase[random.Next(lowercase.Length)];
            password[1] = uppercase[random.Next(uppercase.Length)];
            password[2] = numbers[random.Next(numbers.Length)];

            if (includeSpecialChars)
            {
                password[3] = specialChars[random.Next(specialChars.Length)];
            }

            // Fill the rest with random characters from the full set
            for (int i = includeSpecialChars ? 4 : 3; i < length; i++)
            {
                password[i] = charSet[random.Next(charSet.Length)];
            }

            // Shuffle the password to avoid predictable patterns
            for (int i = password.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (password[i], password[j]) = (password[j], password[i]);
            }

            return new string(password);
        }

        /// <summary>
        /// Validate password strength.
        /// Like checking if a new access code meets security requirements.
        /// </summary>
        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Basic strength requirements
            var hasMinLength = password.Length >= 8;
            var hasMaxLength = password.Length <= 128; // Prevent extremely long passwords
            var hasLowercase = password.Any(char.IsLower);
            var hasUppercase = password.Any(char.IsUpper);
            var hasNumbers = password.Any(char.IsDigit);
            var hasSpecialChars = password.Any(c => !char.IsLetterOrDigit(c));

            // Password must meet all basic criteria
            return hasMinLength && hasMaxLength && hasLowercase && 
                   hasUppercase && hasNumbers && hasSpecialChars;
        }

        /// <summary>
        /// Get password strength score (0-100).
        /// Like rating how secure an access code is on a scale.
        /// </summary>
        public int GetPasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return 0;

            int score = 0;

            // Length scoring
            if (password.Length >= 8) score += 20;
            if (password.Length >= 12) score += 10;
            if (password.Length >= 16) score += 10;

            // Character variety scoring
            if (password.Any(char.IsLower)) score += 10;
            if (password.Any(char.IsUpper)) score += 10;
            if (password.Any(char.IsDigit)) score += 15;
            if (password.Any(c => !char.IsLetterOrDigit(c))) score += 15;

            // Pattern penalties
            if (HasRepeatingCharacters(password)) score -= 10;
            if (HasSequentialCharacters(password)) score -= 10;
            if (IsCommonPassword(password)) score -= 20;

            return Math.Max(0, Math.Min(100, score));
        }

        /// <summary>
        /// Check for repeating characters in password.
        /// </summary>
        private static bool HasRepeatingCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (password[i] == password[i + 1] && password[i] == password[i + 2])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check for sequential characters in password.
        /// </summary>
        private static bool HasSequentialCharacters(string password)
        {
            var lower = password.ToLower();
            for (int i = 0; i < lower.Length - 2; i++)
            {
                if (lower[i + 1] == lower[i] + 1 && lower[i + 2] == lower[i] + 2)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if password is commonly used.
        /// </summary>
        private static bool IsCommonPassword(string password)
        {
            var commonPasswords = new[]
            {
                "password", "123456", "password123", "admin", "qwerty",
                "letmein", "welcome", "monkey", "1234567890", "password1"
            };

            return commonPasswords.Contains(password.ToLower());
        }
    }
}
