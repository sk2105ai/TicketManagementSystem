using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using TicketMonitoringSystem.Models;

namespace TicketMonitoringSystem.Services
{
    public class UserService
    {
        private readonly string _userFilePath;
        private static readonly object _fileLock = new object();

        public UserService()
        {
            _userFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "users.json");
            EnsureUserFileExists();
        }

        private void EnsureUserFileExists()
        {
            var directory = Path.GetDirectoryName(_userFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_userFilePath))
            {
                var userCollection = new UserCollection();
                string jsonString = JsonSerializer.Serialize(userCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_userFilePath, jsonString);
            }
        }

        public bool RegisterUser(User user)
        {
            lock (_fileLock)
            {
                var users = GetAllUsers();

                // Check if username or email already exists
                if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase) ||
                                   u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }

                // Hash the password before storing
                user.HashPassword();

                // Add user
                users.Add(user);
                SaveUsers(users);

                return true;
            }
        }

        public User Authenticate(string username, string password)
        {
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            // Verify password
            if (user != null && user.VerifyPassword(password))
            {
                return user;
            }

            return null;
        }

        public List<User> GetAllUsers()
        {
            lock (_fileLock)
            {
                if (File.Exists(_userFilePath))
                {
                    string jsonString = File.ReadAllText(_userFilePath);
                    var userCollection = JsonSerializer.Deserialize<UserCollection>(jsonString);
                    return userCollection?.Users ?? new List<User>();
                }
                return new List<User>();
            }
        }

        private void SaveUsers(List<User> users)
        {
            lock (_fileLock)
            {
                var userCollection = new UserCollection { Users = users };
                string jsonString = JsonSerializer.Serialize(userCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_userFilePath, jsonString);
            }
        }

        public bool IsUsernameTaken(string username)
        {
            var users = GetAllUsers();
            return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsEmailTaken(string email)
        {
            var users = GetAllUsers();
            return users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }
}