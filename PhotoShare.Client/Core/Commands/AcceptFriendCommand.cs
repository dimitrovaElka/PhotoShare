namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class AcceptFriendCommand
    {
        // AcceptFriend <username1> <username2>
        public static string Execute(string[] data)
        {
            if (data.Length < 3)
            {
                throw new ArgumentException($"Invalid command parameters!");
            }
            string acceptedFriendUsername = data[1];
            string requestUsername = data[2];

            using (var context = new PhotoShareContext())
            {
                var reqUser = context.Users
                    .Include(u => u.FriendsAdded)
                    .ThenInclude(fa => fa.Friend)
                    .FirstOrDefault(u => u.Username == requestUsername);
                if (reqUser == null)
                {
                    throw new ArgumentException($"{requestUsername} not found!");
                }
                var acceptedFriend = context.Users
                    .Include(u => u.AddedAsFriendBy)
                    .ThenInclude(fa => fa.Friend)
                    .FirstOrDefault(u => u.Username == acceptedFriendUsername);
                if (acceptedFriend == null)
                {
                    throw new ArgumentException($"{acceptedFriendUsername} not found!");
                }

                bool alreadyAdded = reqUser.FriendsAdded.Any(u => u.Friend == acceptedFriend);
                bool accepted = acceptedFriend.AddedAsFriendBy.Any(u => u.Friend == reqUser);
                if (alreadyAdded && accepted)
                {
                    throw new InvalidOperationException($"{acceptedFriendUsername} is already a friend to {requestUsername}");
                }
                if (!alreadyAdded)
                {
                    throw new InvalidOperationException($"{acceptedFriendUsername} has not added  {requestUsername} as a friend");
                }
                reqUser.AddedAsFriendBy.Add(
                    new Friendship()
                    {
                        User = acceptedFriend,
                        Friend = reqUser
                    });
                context.SaveChanges();
            }

            return $"{acceptedFriendUsername} accepted {requestUsername} as a friend";
        }
    }
}
