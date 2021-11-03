using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using static System.Console;
using static BulletinBored.Utils;


namespace BulletinBored
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Category> Category { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=laptop-9gj2bhv1;Initial Catalog=BulletinBored;Integrated Security=SSPI");
        }
    }

    public class User
    {
        public int ID { get; set; }
        [MaxLength(12), Required]
        public string UserName { get; set; }
        [MaxLength(12), Required]
        public string PassWord { get; set; }
        public List<Post> UserPosts { get; set; }
    }
    public class Post
    {
        public int ID { get; set; }
        [MaxLength(255),Required]
        public string PostHeading { get; set; }
        public string PostContent { get; set; }
        public List<User> UserLikes { get; set; }
        public List<Category> categories { get; set; }
        
    }
    public class Category
    {
        public int ID { get; set; }
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
        public static AppDbContext database = new AppDbContext();
        public static User currentUser = new User();
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            WriteHeading("Welcome to Bulletin Bored - for when you got nothing better to do.");


            using (database)
            {
                bool running = true;
                while (running)
                {
                    int choice = ShowMenu("", new[] { "Sign In", "Create Account" });

                    if (choice == 0) SignIn();
                    else if (choice == 1) CreateAccount();

                    if (database.User.AsNoTracking().Any(u => u.UserName == currentUser.UserName))
                    {
                        running = false;
                    }
                }

                WriteLine("TEst");
                ReadKey();

                //    int selected = ShowMenu("What do you want to do?", new[] {
                //    "List Movies",
                //    "Add Movie",
                //    "Delete Movie",
                //    "Load Movies from CSV File",
                //    "List Screenings",
                //    "Add Screening",
                //    "Delete Screening",
                //    "Exit"
                //});
                    //Console.Clear();

                    //if (selected == 0) ListMovies();
                    //else if (selected == 1) AddMovie();
                    //else if (selected == 2) DeleteMovie();
                    //else if (selected == 3) LoadMoviesFromCSVFile();
                    //else if (selected == 4) ListScreenings();
                    //else if (selected == 5) AddScreening();
                    //else if (selected == 6) DeleteScreening();
                    //else running = false;

                    //Console.WriteLine();
                
            }
        }

        private static void CreateAccount()
        {
            string userName = ReadString("Choose your username: ");
            string password = ReadString("Choose your password: ");

            var user = new User { UserName = userName, PassWord = password };

            database.User.Add(user);
            database.SaveChanges();

            WriteLine("Account created.");
            WriteLine($"You are logged in as {user.UserName}.");
            currentUser = user;

        }

        private static void SignIn()
        {
           string userName = ReadString("Enter Username: ");
           string passWord = ReadString("Enter Password: ");
            int tries = 3;

           
            if (!database.User.AsNoTracking().Any(u => u.UserName == userName))
            {
                WriteLine("This username does not exist.");
            }
            else
            {
                while (tries > 0)
                {
                    var user = database.User.Single(u => u.UserName == userName);
                    if (user.PassWord != passWord)
                    {
                        WriteLine($"Wrong Password. You have {tries} tries left.");
                        tries--;
                        passWord = ReadString("Enter Password: ");
                    }
                    else 
                    {
                        WriteLine($"You are logged in as {user.UserName}.");
                        currentUser = user;
                        return;
                    } 
                }
                Clear();
                WriteLine("Program shutting down due to failed log in. Press any key to continue.");
                ReadKey();
                Environment.Exit(0);
            }
            
            


            
           
            
        }
    }

    public static class Utils
    {
        public static string ReadString(string prompt)
        {
            Console.Write(prompt + " ");
            string input = Console.ReadLine();
            return input;
        }

        public static int ReadInt(string prompt)
        {
            Console.Write(prompt + " ");
            int input = int.Parse(Console.ReadLine());
            return input;
        }

        public static DateTime ReadDate(string prompt)
        {
            Console.WriteLine(prompt);
            int year = ReadInt("Year:");
            int month = ReadInt("Month:");
            int day = ReadInt("Day:");
            var date = new DateTime(year, month, day);
            return date;
        }

        public static DateTime ReadFutureDate(string prompt)
        {
            var dates = new[]
            {
                DateTime.Now.Date,
                DateTime.Now.AddDays(1).Date,
                DateTime.Now.AddDays(2).Date,
                DateTime.Now.AddDays(3).Date,
                DateTime.Now.AddDays(4).Date,
                DateTime.Now.AddDays(5).Date,
                DateTime.Now.AddDays(6).Date,
                DateTime.Now.AddDays(7).Date
            };
            var wordOptions = new[] { "Today", "Tomorrow" };
            var nameOptions = dates.Skip(2).Select(d => d.DayOfWeek.ToString());
            var options = wordOptions.Concat(nameOptions);
            int daysAhead = ShowMenu(prompt, options.ToArray());
            var selectedDate = dates[daysAhead];
            return selectedDate;
        }

        public static void WriteHeading(string text)
        {
            Console.WriteLine(text);
            string underline = new string('-', text.Length);
            Console.WriteLine(underline);
        }

        public static int ShowMenu(string prompt, string[] options)
        {
            if (options == null || options.Length == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty array of options.");
            }

            Console.WriteLine(prompt);

            int selected = 0;

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                // If this is not the first iteration, move the cursor to the first line of the menu.
                if (key != null)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = Console.CursorTop - options.Length;
                }

                // Print all the options, highlighting the selected one.
                for (int i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine("- " + option);
                    Console.ResetColor();
                }

                // Read another key and adjust the selected value before looping to repeat all of this.
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Length - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }
            }

            // Reset the cursor and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }
}
    

