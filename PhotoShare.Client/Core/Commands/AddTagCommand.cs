namespace PhotoShare.Client.Core.Commands
{

    using Models;
    using Data;
    using Utilities;
    using System.Linq;
    using System;

    public class AddTagCommand
    {
        // AddTag <tag>

        public static string Execute(string[] data)
        {
            if (data.Length < 2)
            {
                throw new ArgumentException($"Command {data[0]} not valid!");
            }
            string tag = data[1].ValidateOrTransform();

            using (PhotoShareContext context = new PhotoShareContext())
            {
                IsLogged.IsLoggedIn(context);

                if (context.Tags.Any(t => t.Name == tag))
                {
                    throw new ArgumentException($"Tag {tag} exists!");
                }
                context.Tags.Add(new Tag
                {
                    Name = tag
                });

                context.SaveChanges();
            }

            return "Tag " + tag + " was added successfully!";
        }
    }
}
