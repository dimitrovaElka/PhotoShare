namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class UploadPictureCommand
    {
        // UploadPicture <albumName> <pictureTitle> <pictureFilePath>
        public static string Execute(string[] data)
        {
            if (data.Length < 4)
            {
                throw new ArgumentException($"Command {data[0]} not valid!");
            }
            string albumName = data[1];
            string pictureTitle = data[2];
            string pictureFilePath = data[3];
            using (var context = new PhotoShareContext())
            {
                var loggedUser = IsLogged.IsLoggedIn(context);
  
                var album = context.Albums
                    .Include(u => u.AlbumRoles)
                    .ThenInclude(x => x.User)
                    .FirstOrDefault(a => a.Name == albumName);

                var albumOwner = album.AlbumRoles
                    .FirstOrDefault(x => x.Role == Role.Owner).User;
                if (loggedUser != albumOwner)
                {
                    throw new InvalidOperationException("Invalid credentials!");
                }
                if (album == null)
                {
                    throw new ArgumentException($"Album {albumName} not found!");
                }
                Picture picture = new Picture()
                {
                    Path = pictureFilePath,
                    Title = pictureTitle,
                    Album = album
                };
                album.Pictures.Add(picture);
                context.SaveChanges();
            }

            return $"Picture {pictureTitle} added to {albumName}!";
        }
    }
}
