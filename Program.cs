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
        public DbSet<Like> Like { get; set; }

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
        public List<Post> Posts { get; set; }
        public List<Like> Likes { get; set; }

        public User()
        {
            Likes = new List<Like>();
        }
    }
    public class Post
    {
        public int ID { get; set; }
        [MaxLength(255),Required]
        public string PostHeading { get; set; }
        public string PostContent { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public List<Category> Categories { get; set; }
        public List<Like> Likes { get; set; }

        public Post()
        {
            Likes = new List<Like>();
        }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
       
    }

    public class Like
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }
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
                PopulateCategory();

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
                WriteLine();

                running = true;
                while (running)
                {
                    Clear();
                    WriteHeading("Main Menu");
                    int selected = ShowMenu("", new[] {
                    "My Posts",
                    "Create Post",
                    "Delete Post",
                    "Most Recent Posts",
                    "Most Popular Posts",
                    "Posts by Category",
                    "All Posts",
                    "Search",
                    "Quit"
                });
                    Clear();

                    if (selected == 0) ListUserPosts();
                    else if (selected == 1) CreatePost();
                    else if (selected == 2) DeletePost();
                    else if (selected == 3) ListMostRecent();
                    else if (selected == 4) ListMostLiked();
                    else if (selected == 5) ListByCategory();
                    else if (selected == 6) ShowAllPosts();
                    else if (selected == 7) SearchPosts();
                    else running = false;

                    WriteLine(); 
                }

            }
        }

        private static void ShowAllPosts()
        {
            foreach (var post in database.Post)
            {
                ShowPostInfo(post);
            }
            WriteLine("Press any key to continue");
            ReadKey();
        }

        private static void SearchPosts()
        {
            ReadString("What do you wish to search for?");
        }

        private static void ListByCategory()
        {
            WriteLine();

            int choice1 = ShowMenu("", database.Category.OrderBy(c => c.Name).Select(c => c.Name).ToArray());

            WriteLine();

            var category = database.Category.OrderBy(c => c.Name).Skip(choice1).First();

            var posts = database.Post.OrderBy(p => p.PostHeading).Where(p => p.Categories.Contains(category)).Select(p => p.PostHeading).ToArray();
            int choice2 = ShowMenu($"Here are all posts tagged with {category.Name}.", posts);

            var postHeading = posts.Skip(choice2).First();

            var post = database.Post.Where(p => p.PostHeading == postHeading).Single();

            ShowPostInfo(post);
            LikePost(post);
        }

        private static void ListMostLiked()
        {
            var posts = database.Post.AsNoTracking().OrderByDescending(p => p.Likes.Count()).Select(p => p.PostHeading).Take(5).ToArray();
            int choice = ShowMenu("Here are the 5 most liked posts ordered by number of likes, choose to read.", posts);

            var postToShow = database.Post.AsNoTracking().OrderByDescending(p => p.Likes.Count()).Take(5).Skip(choice).First();

            ShowPostInfo(postToShow);

            LikePost(postToShow);
        }

        private static void ListMostRecent()
        {
            var posts = database.Post.AsNoTracking().OrderByDescending(p => p.Date).Select(p => p.PostHeading).Take(5).ToArray();
            int choice = ShowMenu("Here are the 5 latest posts ordered by date, choose to read.", posts);

            var postToShow = database.Post.AsNoTracking().OrderByDescending(p => p.Date).Take(5).Skip(choice).First();

            ShowPostInfo(postToShow);

            LikePost(postToShow);
        }
        private static void ShowPostInfo(Post post)
        {
                int postID = post.ID;
                var like = database.Like.Select(x => x.Post).Where(p => p.ID == postID).Count();
                WriteLine();
                WriteLine($"- {post.PostHeading}");
                WriteLine($"- {post.PostContent}");
                WriteLine($"- {post.Date:g}");
                if (like == 0)
                {
                    WriteLine("This post has no likes yet.");
                }
                else
                {
                    WriteLine($"- {like} likes.");
                }
                WriteLine(); 
        }

        private static void LikePost(Post post)
        {
            int postID = post.ID;
            int userID = currentUser.ID;

            bool liked = database.Like.Select(l => l.User).Any(u => u.ID == postID);

            int choice = ShowMenu("Do you like this post?", new[] { "Yes", "No" });

            if (choice == 0)
            {
                if (liked)
                {
                    WriteLine("You already like this post");
                    ReadKey();
                }
                else
                {
                    var like = new Like
                    {
                        Date = DateTime.Now,
                    };

                    var currentPost = database.Post.Where(p => p.ID == post.ID).Single();

                    currentPost.Likes.Add(like);
                    currentUser.Likes.Add(like);
                    database.Like.Add(like);
                    database.SaveChanges(); 
                }
            }
            else { }
        }

        private static void DeletePost()
        {
            int choice = ShowMenu("Which post would you like to delete?", currentUser.Posts.Select(up => up.PostHeading).ToArray());

            var postToDelete = currentUser.Posts.Skip(choice).First();

            database.Remove(postToDelete);
            database.SaveChanges();
        }

        public static List<Category> ChooseCategories()
        {
            
            int counter = 1;
            var chosenCategories = new List<Category>();

            while (true)
            {
                int choice = ShowMenu($"Choose Category {counter}", database.Category.Select( c => c.Name).ToArray());
                counter++;
                
                chosenCategories.Add(database.Category.Skip(choice).First()); 
               

                int yesNo = ShowMenu("Would you like to add more categories?", new[] { "Yes", "No" });
                if (yesNo == 0) { }
                else if (yesNo == 1) { break; } 
            }

                return chosenCategories; 
        }

        private static void CreatePost()
        {
            Clear();
            string heading = ReadString("What's the heading?");
            string content = ReadString("What's your message?");
            var categories = ChooseCategories();
            
            DateTime date = DateTime.Now;

            

            var post = new Post
            {
                PostHeading = heading,
                PostContent = content,
                Categories = categories,
                Date = date,
            };

            currentUser.Posts.Add(post);
            database.SaveChanges();
        }

        private static void ListUserPosts()
        {
            if (currentUser.Posts.Count() == 0)
            {
                WriteLine("You have no posts to show");
            }
            else 
            {
                WriteHeading($"Posts by {currentUser.UserName}");
                foreach (var post in currentUser.Posts)
                {
                    ShowPostInfo(post);
                    
                }
                WriteLine("Press any key to continue");
                ReadKey();
            }
        }

        private static void CreateAccount()
        {
            while (true)
            {
                string userName = ReadString("Choose your username: ");
                string password = ReadString("Choose your password: ");

                if (database.User.AsNoTracking().Any(u => u.UserName == userName))
                {
                    WriteLine("This username is taken, please try something else.");
                }
                else 
                {
                    var user = new User { UserName = userName, PassWord = password };
                    database.User.Add(user);
                    database.SaveChanges();

                    WriteLine("Account created.");
                    WriteLine($"You are logged in as {user.UserName}.");
                    currentUser = database.User.Include(u => u.Posts).Where(u => u.UserName == userName).Single();

                    break; 
                }
            }
            

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
                        currentUser = database.User.Include(u => u.Posts).Where(u => u.UserName == userName).Single();
                        return;
                    } 
                }
                Clear();
                WriteLine("Program shutting down due to failed log in. Press any key to continue.");
                ReadKey();
                Environment.Exit(0);
            }
        }

        private static void PopulateCategory()
        {
            if (database.Category.Count() != 0)
            {

            }
            else
            {
                string[] categories = { "School", "Gaming", "Food", "Humor", "Event", "Whatever" };
                foreach (var name in categories)
                {
                    var category = new Category { Name = name };
                    database.Category.Add(category);
                }
                database.SaveChanges();
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
    

