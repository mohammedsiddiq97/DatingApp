﻿using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Data
{
    public class DataContext : IdentityDbContext<User,Role,int,IdentityUserClaim<int>,UserRole,IdentityUserLogin<int>
        ,IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {}
        public DbSet<Value> Value { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<Role>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            builder.Entity<Like>()
                    .HasKey(k => new { k.LikerId, k.LikeeId });
            builder.Entity<Like>()
                    .HasOne(u=> u.Likee)
                    .WithMany(u => u.Liker)
                    .HasForeignKey(u=> u.LikeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>()
                  .HasOne(u => u.Liker)
                  .WithMany(u => u.Likees)
                  .HasForeignKey(u => u.LikerId)
                  .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Message>()
             .HasOne(u => u.Sender)
             .WithMany(u => u.MessagesSent)
             .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Message>()
           .HasOne(u => u.Recipient)
           .WithMany(u => u.MessagesReceived)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
