using InventoryManagementSystem.Data.Model;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace InventoryManagementSystem.Data
{
    public class UserService
    {
        // Details of admin to be created during initialization
        public const string SeedUsername = "admin1";
        public const string SeedEmail = "admin1@gmail.com";
        public const string SeedPassword = "admin1";

        // Registering Admin initially when the program is executed
        public static void SeedUsers()
        {
            // Getting all users from users file
            var users = GetAllUsers();

            // if there is no any user in users file, then an admin will be registered for the first time
            if (users.Count() == 0)
            {
                RegisterUser(Guid.Empty, SeedUsername, SeedEmail, SeedPassword, Role.Admin);
            }
        }

        // Method for geting all users from users.json file
        public static List<User> GetAllUsers()
        {
            // Getting users file path
            string usersFilePath = Utils.GetUsersFilePath();

            // If the users file does not exists, then the user list will be returned
            if (!File.Exists(usersFilePath))
            {
                return new List<User>();
            }

            // Reading all the data from users file
            var json = File.ReadAllText(usersFilePath);

            // Convert json file to list of users
            return JsonSerializer.Deserialize<List<User>>(json);
        }

        // Method for saving all users
        private static void SaveAllUsers(List<User> users)
        {
            var filesDirectoryPath = Utils.GetFilesDirectory();
            var usersFilePath = Utils.GetUsersFilePath();

            // If the directory does not exists, the the directory will be created
            if (!Directory.Exists(filesDirectoryPath))
            {
                Directory.CreateDirectory(filesDirectoryPath);
            }

            // Converting list of users into json data
            var json = JsonSerializer.Serialize(users);

            // Writing the above json data into the users file
            File.WriteAllText(usersFilePath, json);
        }

        // Method for creating or registering users
        public static User RegisterUser(Guid userId, string username, string email, string password, Role role)
        {
            // Getting all users from user file
            var allUsers = GetAllUsers();

            // Checking if the passed username already exists
            bool usernameExists = allUsers.Any(x => x.Username == username);

            // If the passed username exists, error message will be thrown
            if (usernameExists)
            {
                throw new Exception("Username already exists!");
            }

            // Geting total number of admin
            int countAdmin = allUsers.FindAll(x => x.Role == Role.Admin).Count();

            // If already two admin exists, then error message will be shown as there cannot be more than two admins
            if (countAdmin == 2 && role == Role.Admin)
            {
                throw new Exception("SORRY! There cannot be more than two Admins.");
            }

            // Creating a user
            var user = new User()
            {
                Username = username,
                Email = email,
                PasswordHash = Utils.HashSecret(password),
                Role = role,
                CreatedBy = userId
            };

            // Adding and saving the user in users file
            allUsers.Add(user);
            SaveAllUsers(allUsers);

            // returning user
            return user;
        }

        // Login method
        public static User Login(string username, string password)
        {
            var loginErrorMessage = "Invalid Username or Password!";
            List<User> users = GetAllUsers();
            User user = users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                throw new Exception(loginErrorMessage);
            }

            bool passwordIsValid = Utils.VerifyHash(password, user.PasswordHash);

            if (!passwordIsValid)
            {
                throw new Exception(loginErrorMessage);
            }

            // Checking Date and time before logging into system for STAFFs
            if (user.Role == Role.Staff)
            {
                var todayWeek = DateTime.Now.ToString("dddd");
                int todayTime = Convert.ToInt32(DateTime.Now.ToString("HH"));

                if(todayTime < 9 || todayTime > 16)
                {
                    throw new Exception("Sorry! You can only access between 9:00 AM - 4:00 PM. ");
                }
                else
                {
                    if ((todayWeek == "Saturday" || todayWeek == "Sunday"))
                    {
                        throw new Exception("Ops! System can only be accessed Monday to Friday.");
                    }
                }
            }

            return user;

        }
    }
}
