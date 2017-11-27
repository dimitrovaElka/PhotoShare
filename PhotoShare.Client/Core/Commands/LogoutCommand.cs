
namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using System;
    using System.Linq;

    public class LogoutCommand
    {
        // logout
        public static string Execute()
        {
            var userName = "";
            using (var context = new PhotoShareContext())
            {
                var session = context.Sessions
                    .Include(s => s.User)
                    .FirstOrDefault(s => s.LoggedOut == null);
                if (session == null)
                {
                    throw new InvalidOperationException("You should log in first in order to logout.");
                }
                userName = session.User.Username;
                session.LoggedOut = DateTime.Now;
                context.SaveChanges();

            }
            return $"User {userName} successfully logged out!";
        }
    }
}
