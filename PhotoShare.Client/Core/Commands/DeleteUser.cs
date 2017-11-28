namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;

    using Data;

    public class DeleteUser
    {
        // DeleteUser <username>
        public static string Execute(string[] data)
        {
            string username = data[1];
            using (PhotoShareContext context = new PhotoShareContext())
            {
                var loggedUser = IsLogged.IsLoggedIn(context);
                var user = context.Users.FirstOrDefault(u => u.Username == username);
                if (loggedUser != user)
                {
                    throw new InvalidOperationException("Invalid credentials!");
                }
                if (user == null)
                {
                    throw new InvalidOperationException($"User with {username} was not found!");
                }

                // TODO: Delete User by username (only mark him as inactive)
                if (user.IsDeleted.Value)
                {
                    throw new InvalidOperationException($"User {username} is already deleted!");
                }
                user.IsDeleted = true;
                //var userSession = user.Sessions.FirstOrDefault(s => s.LoggedOut == null);
                //userSession.LoggedOut = DateTime.Now;
                context.SaveChanges();

                return $"User {username} was deleted from the database!";
            }
        }
    }
}
