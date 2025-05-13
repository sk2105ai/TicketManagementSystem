using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace TicketMonitoringSystem.Models
{
    public class User
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [JsonIgnore] // Prevent password serialization
        public string Password { get; set; }

        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Method to hash password
        public void HashPassword()
        {
            // Generate salt
            Salt = GenerateSalt();

            // Hash password with salt
            PasswordHash = HashPasswordWithSalt(Password, Salt);

            // Clear plain text password
            Password = null;
        }

        // Verify password
        public bool VerifyPassword(string inputPassword)
        {
            if (string.IsNullOrEmpty(PasswordHash) || string.IsNullOrEmpty(Salt))
                return false;

            string hashedInputPassword = HashPasswordWithSalt(inputPassword, Salt);
            return hashedInputPassword == PasswordHash;
        }

        // Generate salt
        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Hash password with salt
        private static string HashPasswordWithSalt(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // Combine password and salt
                string passwordWithSalt = password + salt;

                // Convert to byte array and hash
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));

                // Convert to string
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }

    public enum UserRole
    {
        User,
        Admin
    }

    public class UserCollection
    {
        public List<User> Users { get; set; } = new List<User>();
    }
}
