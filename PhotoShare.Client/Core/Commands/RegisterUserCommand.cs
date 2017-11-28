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
                var existingSession = context.Sessions.Any(s => s.LoggedOut == null);
                if (existingSession)
                {
                    throw new InvalidOperationException("Invalid credentials!");
                }
                var existingUser = context.Users
                    .Where(u => u.Username == username)
                    .FirstOrDefault();
                if (existingUser != null && existingUser.IsDeleted == false)
                {
                    throw new InvalidOperationException($"User {username} is already taken!");
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
                if (existingUser != null && existingUser.IsDeleted == true)
                {
                    existingUser.IsDeleted = false;
                    existingUser.Password = password;
                    existingUser.Email = email;
                    context.Users.Update(existingUser);
                }
                else
                {
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
                }
                context.SaveChanges();
            }

            return "User " + username + " was registered successfully!";
        }
    }
}
