namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using System.Linq;
    using System;
    using PhotoShare.Models;
    using Microsoft.EntityFrameworkCore;

    public class AddFriendCommand
    {
        // AddFriend <username1> <username2>
        public static string Execute(string[] data)
        {
            string reqUsername = data[1];
            string addedFriendUsername = data[2];

            using (var context = new PhotoShareContext())
            {
                var reqUser = context.Users
                    .Include(u => u.FriendsAdded)
                    .ThenInclude(fa => fa.Friend)
                    .FirstOrDefault(u => u.Username == reqUsername);
                if (reqUser == null)
                {
                    throw new ArgumentException($"{reqUsername} not found!");
                }
                var addedFriend = context.Users
                    .Include(u => u.FriendsAdded)
                    .ThenInclude(fa => fa.Friend)
                    .FirstOrDefault(u => u.Username == addedFriendUsername);
                if (addedFriend == null)
                {
                    throw new ArgumentException($"{addedFriendUsername} not found!");
                }

                bool alreadyAdded = reqUser.FriendsAdded.Any(u => u.Friend == addedFriend);
                bool accepted = addedFriend.FriendsAdded.Any(u => u.Friend == reqUser);
                if (alreadyAdded && accepted)
                {
                    throw new InvalidOperationException($"{addedFriendUsername} is already a friend to {reqUsername}");
                }
                if (alreadyAdded && !accepted)
                {
                    throw new InvalidOperationException($"{reqUser} is already sent invite to {addedFriend}");
                }
                if (!alreadyAdded && accepted)
                {
                    throw new InvalidOperationException($"{addedFriend} is already sent invite to {reqUser}");
                }
                reqUser.FriendsAdded.Add(
                    new Friendship()
                    {
                        User = reqUser,
                        Friend = addedFriend
                    });
                context.SaveChanges();
            }
            return $"Friend {addedFriendUsername} added to {reqUsername}";
        }
    }
}
