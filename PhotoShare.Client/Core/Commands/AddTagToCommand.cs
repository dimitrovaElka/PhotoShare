namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class AddTagToCommand 
    {
        // AddTagTo <albumName> <tag>
        public static string Execute(string[] data)
        {
            var albumName = data[1];
            var tag = data[2];
            using (var context = new PhotoShareContext())
            {
                var loggedUser = IsLogged.IsLoggedIn(context);

                Album album = context.Albums
                    .Include(a => a.AlbumRoles)
                    .ThenInclude(ar => ar.User)
                    .FirstOrDefault(a => a.Name == albumName);
                User albumOwner = album.AlbumRoles               
                    .FirstOrDefault(x => x.Role == Role.Owner)
                    .User;
                if (loggedUser != albumOwner)
                {
                    throw new InvalidOperationException("Invalid credentials!");
                }
                Tag tagToAdd = context.Tags.FirstOrDefault(t => t.Name == "#" + tag);
                if (album == null || tagToAdd == null)   
                {
                    throw new ArgumentException($"Either {tag} or {albumName} do not exist!");
                }

                AlbumTag albumTag = new AlbumTag()
                {
                    Album = album,
                    Tag = tagToAdd
                };
                context.AlbumTags.Add(albumTag);
                context.SaveChanges();
            }
            return $"Tag {tag} added to {albumName}!";
        }
    }
}
