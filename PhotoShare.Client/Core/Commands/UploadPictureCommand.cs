namespace PhotoShare.Client.Core.Commands
{
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
                var album = context.Albums
                    .FirstOrDefault(a => a.Name == albumName);
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
