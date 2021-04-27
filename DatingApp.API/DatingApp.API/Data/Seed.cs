using DatingApp.API.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedDatabase(DatingContext context)
        {
            if (!context.Users.Any())
            {
                var data = File.ReadAllText("Data/SeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(data);

                foreach (var user in users)
                {
                    CalculatePasswordHashAndSalt("password", out byte[] passwordHash, out byte[] passwordSalt);
                    user.UserName = user.UserName.ToLower();
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;

                    context.Add(user);
                }

                context.SaveChanges();
            }
        }

        private static void CalculatePasswordHashAndSalt(
            string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
