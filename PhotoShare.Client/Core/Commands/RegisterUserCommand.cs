namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Models;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class RegisterUserCommand
    {
        // RegisterUser <username> <password> <repeat-password> <email> 
        public static string Execute(string[] data)
        {
            if (data.Length < 5)
            {
                throw new ArgumentException($"Command {data[0]} not valid!");
            }
            string username = data[1];
            string password = data[2];
            string repeatPassword = data[3];
            string email = data[4];

            using (PhotoShareContext context = new PhotoShareContext())
            {
                var existingUser = context.Users
                    .AsNoTracking()
                    .Where(u => u.Username == username)
                    .FirstOrDefault();
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"Username {username} is already taken!");
                }
                if (password.Length < 6 || password.Length > 50 ||
                            !password.Any(c => Char.IsDigit(c)) || 
                            !password.Any(c => Char.IsLower(c)))
                {
                    throw new ArgumentException("Invalid password");
                }
                if (password != repeatPassword)
                {
                    throw new ArgumentException("Passwords do not match!");
                }

                User user = new User
                {
                    Username = username,
                    Password = password,
                    Email = email,
                    IsDeleted = false,
                    RegisteredOn = DateTime.Now,
                    LastTimeLoggedIn = DateTime.Now
                };
                context.Users.Add(user);
                context.SaveChanges();
            }

            return "User " + username + " was registered successfully!";
        }
    }
}
