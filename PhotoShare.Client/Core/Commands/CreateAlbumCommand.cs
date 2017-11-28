namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CreateAlbumCommand
    {
        // CreateAlbum <username> <albumTitle> <BgColor> <tag1> <tag2>...<tagN>
        public static string Execute(string[] data)
        {
            string username = data[1];
            string albumTitle = data[2];
            if (data.Length < 3)
            {
                throw new ArgumentException($"Color {String.Empty} not found!");
            }
            string color = data[3];
            string[] tags = data.Skip(4).ToArray();
            var result = "";
            using (var context = new PhotoShareContext())
            {
                var loggedUser = IsLogged.IsLoggedIn(context);
                var existingUser = context.Users
                    .FirstOrDefault(u => u.Username == username);
                if (loggedUser != existingUser)
                {
                    throw new InvalidOperationException("Invalid credentials!");
                }
                if (existingUser == null)
                {
                    throw new ArgumentException($"User {username} not found!");
                }

                if (context.Albums.Any(a => a.Name == albumTitle))
                {
                    throw new ArgumentException($"Album {albumTitle} exists!");
                }
                Album album = new Album()
                {
                    Name = albumTitle
                };
                try
                {
                    Color colorValue = (Color)Enum.Parse(typeof(Color), color);
                    album.BackgroundColor = colorValue;
                    context.Albums.Add(album);

                    AlbumRole albumRole = new AlbumRole()
                    {
                        User = existingUser,
                        Album = album,
                        Role = Role.Owner
                    };
                    context.AlbumRoles.Add(albumRole);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($"Color {color} not found!");
                }
                foreach (var tag in tags)
                {
                    var tagExists = context.Tags
                        .Where(t => t.Name == "#" + tag)
                        .FirstOrDefault();
                    if (tagExists == null)
                    {
                        throw new ArgumentException($"Invalid tags!");
                    }
                    AlbumTag albumTag = new AlbumTag()
                    {
                        Album = album,
                        Tag = tagExists
                    };
                    context.AlbumTags.Add(albumTag);
                }
                context.SaveChanges();
                result = $"Album {albumTitle} successfully created!";
            }
            
            return result;
        }
    }
}
