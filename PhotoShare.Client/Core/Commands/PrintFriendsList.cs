namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;
    using System.Text;

    public class PrintFriendsListCommand 
    {
        // PrintFriendsList <username>
        public static string Execute(string[] data)
        {
            StringBuilder result = new StringBuilder();
            // prints all friends of user with given username.
            if (data.Length < 2)
            {
                throw new ArgumentException($"Invalid command parameters!");
            }
            string username = data[1];

            using (var context = new PhotoShareContext())
            {
                User user = context.Users
                    .Include(u => u.FriendsAdded)
                    .ThenInclude(fa => fa.Friend)
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == username);
                    
                if (user == null)
                {
                    throw new ArgumentException($"User {user} not found!");
                }
                var friends = user.FriendsAdded
                    .Select(fa => fa.Friend.Username)
                    .ToArray();
                if (friends.Length == 0)
                {
                    throw new ArgumentException($"No friends for this user. :(");
                }
                result.AppendLine("Friends:");
                foreach (var f in friends)
                {
                    result.AppendLine($"-[{f}]");
                }
            }
            return result.ToString();
        }
    }
}
