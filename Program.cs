using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;


namespace BulletinBored
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Category> Category { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=laptop-9gj2bhv1;Initial Catalog=BulletinBored;Intergrated Security=SSPI");
        }
    }

    public class User
    {
        [MaxLength(12), Required]
        public string UserName { get; set; }
        [MaxLength(12), Required]
        public string PassWord { get; set; }
        public List<Post> UserPosts { get; set; }
        public List<Post> LikedPosts { get; set; }
    }
    public class Post
    {
        [MaxLength(255),Required]
        public string PostHeading { get; set; }
        public string PostContent { get; set; }
        public List<Category> categories { get; set; }
        public List<User> UserLikes { get; set; }
        [Required]
        public User User { get; set; }
    }
    public class Category
    {
        public string School { get; set; }
        public string Humor { get; set; }
        public string Event { get; set; }
        public string Food { get; set; }
        public string Gaming { get; set; }
        public string WhatEver { get; set; }
        [Required]
        public Post Post { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
