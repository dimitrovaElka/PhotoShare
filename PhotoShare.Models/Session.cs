namespace PhotoShare.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class Session
    {
        public int SessionId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime LoggedIn { get; set; }
        public DateTime? LoggedOut { get; set; }

    }
}
