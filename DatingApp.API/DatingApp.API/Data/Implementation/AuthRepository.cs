using DatingApp.API.Data.Interfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.API.Data.Implementation
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatingContext _context;

        public AuthRepository(DatingContext context)
        {
            _context = context;
        }
        public async Task<User> Register(User user, string password)
        {

            CreatePasswordHashAndSalt(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> Login(string userName, string password)
        {
            var user = await _context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == userName);

            if (user == null)
                return null;

            if (!ValidateUserPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;

        }


        public async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        private void CreatePasswordHashAndSalt(
            string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool ValidateUserPassword(
            string password, byte[] userPasswordHash, byte[] userPasswordSalt)
        {
            byte[] hashedPassword;
            using (var hmac = new HMACSHA512(userPasswordSalt))
            {
                hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            for (int i = 0; i < hashedPassword.Length; i++)
            {
                if (userPasswordHash[i] != hashedPassword[i])
                    return false;
            }

            return true;
        }


    }
}
