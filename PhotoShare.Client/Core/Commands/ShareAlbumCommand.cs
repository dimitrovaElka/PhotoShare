namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class ShareAlbumCommand
    {
        // ShareAlbum <albumId> <username> <permission>
        // For example:
        // ShareAlbum 4 dragon321 Owner
        // ShareAlbum 4 dragon11 Viewer
        public static string Execute(string[] data)
        {
            if (data.Length < 4)
            {
                throw new ArgumentException($"Command {data[0]} not valid!");
            }
            int albumId = -1;
            bool albumExist = int.TryParse(data[1], out albumId);
            string username = data[2];
            string inputPermission = data[3];

            Role permission;
            if (!Enum.TryParse(inputPermission, out permission))
            {
                throw new ArgumentException("Permission must be either “Owner” or “Viewer”!");
            }
            using (var context = new PhotoShareContext())
            {
                var user = context.Users
                    .Include(u => u.AlbumRoles)
                    .FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    throw new ArgumentException($"User {username} not found!");
                }
                var album = context.Albums
                    .Include(a => a.AlbumRoles)
                    .FirstOrDefault(a => a.Id == albumId);
                if (album == null)
                {
                    throw new ArgumentException($"Album {albumId} not found!");
                }
                user.AlbumRoles.Add(new AlbumRole()
                {
                    AlbumId = albumId,
                    Role = permission
                });
                context.SaveChanges();
            }


            return $"Username {username} added to album {albumId} ({inputPermission})";
        }
    }
}
