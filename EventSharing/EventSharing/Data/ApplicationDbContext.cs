using EventSharing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventSharing.ViewModels;

namespace EventSharing.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
               .HasMany(u => u.CreatedEvents)
               .WithOne(e => e.Creator);
            builder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany(u => u.CreatedEvents);

            //Soit en cr�ant comme �a soit avec des annotations dans les mod�les
            builder.Entity<Category>()
                .Property(c => c.Name)
                .HasMaxLength(256)
                .IsRequired();
            builder.Entity<Category>()
                .Property(c => c.Description)
                .HasMaxLength(256)
                .IsRequired();

            builder.Entity<Event>()
                .Property(e => e.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Entity<Event>()
                .Property(e => e.Description)
                .IsRequired();
            builder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(256);
        }
        }

    }

