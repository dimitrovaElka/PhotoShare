namespace PhotoShare.Client.Core.Commands
{
    using Models;
    using Data;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using System;

    public class AddTownCommand
    {
        // AddTown <townName> <countryName>
        public static string Execute(string[] data)
        {
            if (data.Length < 3)
            {
                throw new ArgumentException($"Command {data[0]} not valid!");
            }
            string townName = data[1];
            string country = data[2];

            using (PhotoShareContext context = new PhotoShareContext())
            {
                IsLogged.IsLoggedIn(context);

                if (context.Towns.Any(t => t.Name == townName))
                {
                    throw new ArgumentException($"Town {townName} was already added!");
                }
                Town town = new Town
                {
                    Name = townName,
                    Country = country
                };

                context.Towns.Add(town);
                context.SaveChanges();

                return "Town " + townName + " was added succesfully!";
            }
        }


    }
}
