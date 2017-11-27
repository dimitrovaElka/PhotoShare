namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;

    public class ModifyUserCommand
    {
        // ModifyUser <username> <property> <new value>
        // For example:
        // ModifyUser <username> Password <NewPassword>
        // ModifyUser <username> BornTown <newBornTownName>
        // ModifyUser <username> CurrentTown <newCurrentTownName>
        // !!! Cannot change username
        public static string Execute(string[] data)
        {
            string username = data[1];
            string property = data[2].ToLower();
            string newValue = data[3];

            var exceptionMessage = $"Value {newValue} not valid." + Environment.NewLine;
            var errorTown = $"Town {newValue} not found!";
            using (var context = new Data.PhotoShareContext())
            {
                var user = context.Users
                    .Where(u => u.Username == username)
                    .FirstOrDefault();
                if (user == null)
                {
                    throw new ArgumentException($"User {username} not found!");
                }
                switch (property)
                {
                    case "password":
                        if (newValue.Length < 6 ||
                            newValue.Length > 50 ||
                            !newValue.Any(c => Char.IsDigit(c)) ||
                            !newValue.Any(c => Char.IsLower(c)))
                        {
                            throw new ArgumentException(exceptionMessage + "Invalid password");
                        }
                        user.Password = newValue;
                        break;
                    case "borntown":
                        var bornTown = context.Towns
                            .Where(t => t.Name == newValue)
                            .FirstOrDefault();
                        if (bornTown == null)
                        {
                            throw new ArgumentException(exceptionMessage + errorTown);
                        }
                        user.BornTown = bornTown;
                        break;
                    case "currenttown":
                        var currentown = context.Towns
                            .Where(t => t.Name == newValue)
                            .FirstOrDefault();
                        if (currentown == null)
                        {
                            throw new ArgumentException(exceptionMessage + errorTown);
                        }
                        user.CurrentTown = currentown;
                        break;
                    default:
                        throw new ArgumentException($"Property {property} not supported!");
                }
                context.SaveChanges();
                return $"User {username} {property} is {newValue}.";
            }
        }
    }
}
