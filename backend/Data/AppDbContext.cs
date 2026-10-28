using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DjProfile> DjProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var stringListComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            builder.Entity<DjProfile>(entity =>
            {
                // Configures SocialLinks owned entity and satisfies EF Core validation rules
                entity.OwnsOne(p => p.SocialLinks);
                entity.Navigation(p => p.SocialLinks).IsRequired();

                // Equipment collection converter & comparer
                entity.Property(e => e.Equipment)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(stringListComparer);

                // Genres collection converter & comparer
                entity.Property(e => e.Genres)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(stringListComparer);
            });
        }
    }
}