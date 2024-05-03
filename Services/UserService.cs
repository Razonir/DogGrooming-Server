using DogGrooming_Server.Data;
using DogGrooming_Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DogGrooming_Server.Services
{
    public class UserService
    {
        private readonly ServerDBContext _dbContext;

        public UserService(ServerDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // Sign Up User
        public async Task<User> SignUpUser(User user)
        {
            if (await _dbContext.User.AnyAsync(u => u.UserName == user.UserName))
            {
                throw new Exception("משתמש עם שם משתמש זהה כבר קיים במערכת");
            }
            string hashedPassword = GetMd5Hash(user.Password);
            var newUser = new User
            {
                UserName = user.UserName,
                Password = hashedPassword,
                FirstName = user.FirstName,
                Role = "User",
                RegisterDate = DateTime.Now,
            };
            _dbContext.User.Add(newUser);
            await _dbContext.SaveChangesAsync();
            return newUser;
        }

        //Login 
        public async Task<User> Login(Login login)
        {
            string hashedPassword = GetMd5Hash(login.Password);
            return await _dbContext.User.FirstOrDefaultAsync(u => u.UserName == login.UserName && u.Password == hashedPassword);
        }


        // GetUserById
        public async Task<User> GetUserById(int userId)
        {
            return await _dbContext.User.FindAsync(userId);
        }

        private string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }



    }
}
