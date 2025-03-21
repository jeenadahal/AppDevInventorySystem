﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace InventoryManagementSystem.Data.Model
{
    public class Utils
    {
        private const char _segmentDelimiter = ':';

        // Getting path of directory where users, products files are stored
        public static string GetFilesDirectory()
        {
            return "D:\\AD\\InventoryManagementSystem\\wwwroot\\jsonData";
        }

        // Getting path of file where users data are stored
        public static string GetUsersFilePath()
        {
            return Path.Combine(GetFilesDirectory(), "users.json");
        }

        
        public static string HashSecret(string input)
        {
            var saltSize = 16;
            var iterations = 100_000;
            var keySize = 32;
            HashAlgorithmName algorithm = HashAlgorithmName.SHA256;
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

            return string.Join(
                _segmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                iterations,
                algorithm
            );
        }
        public static bool VerifyHash(string input, string hashString)
        {
            string[] segments = hashString.Split(_segmentDelimiter);
            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            HashAlgorithmName algorithm = new(segments[3]);
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                iterations,
                algorithm,
                hash.Length
            );

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }

    }
}
