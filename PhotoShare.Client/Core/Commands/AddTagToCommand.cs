namespace PhotoShare.Client.Core.Commands
{
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
                Album album = context.Albums.FirstOrDefault(a => a.Name == albumName);
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
