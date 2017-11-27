
namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Models;
    using Data;
    using Microsoft.EntityFrameworkCore;
    public class LoginCommand
    {
        // Login <username> <password>
        public static string Execute(string[] data)
        {
            if (data.Length < 3)
            {
                throw new ArgumentException($"Command {data[0]} not valid!");
            }
            string username = data[1];
            string password = data[2];

            using (var context = new PhotoShareContext())
            {
                var user = context.Users
                    .Include(u => u.Sessions)
                    .FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user == null)
                {
                    throw new ArgumentException("Invalid username or password!");
                }
                var session = user.Sessions
                    .Where(s => s.LoggedIn != null && s.LoggedOut == null)
                    .ToArray();
                if (session.Length > 0)
                {
                    throw new ArgumentException("You should logout first!");
                }
                context.Sessions.Add(new Session()
                {
                    User = user
                });
                context.SaveChanges();
            }

            return $"User {username} successfully logged in!";
        }
    }
}
