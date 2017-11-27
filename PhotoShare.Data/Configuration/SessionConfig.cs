namespace PhotoShare.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Models;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;

    public class SessionConfig : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(s => s.SessionId);

            builder.Property(e => e.LoggedIn)
                .HasDefaultValue(DateTime.Now);

            builder.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId);

        }
    }
}
