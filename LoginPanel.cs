using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class User
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
}

public class Program
{
    private static Dictionary<string, User> users = new Dictionary<string, User>();

    public static void Main()
    {
        while (true)
        {
            Console.WriteLine("1. Create account");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
            var option = Console.ReadLine();

            if (option == "1")
            {
                CreateAccount();
            }
            else if (option == "2")
            {
                Login();
            }
            else if (option == "3")
            {
                break;
            }
        }
    }

    private static void CreateAccount()
    {
        Console.Write("Enter a username: ");
        var username = Console.ReadLine();
        Console.Write("Enter a password: ");
        var password = Console.ReadLine();

        var salt = GenerateSalt();
        var passwordHash = ComputeHash(salt, password);

        users[username] = new User { Username = username, PasswordHash = passwordHash, Salt = salt };

        Console.WriteLine("Account created successfully!");
    }

    private static void Login()
    {
        Console.Write("Enter your username: ");
        var username = Console.ReadLine();
        Console.Write("Enter your password: ");
        var password = Console.ReadLine();

        if (users.ContainsKey(username))
        {
            var user = users[username];
            var passwordHash = ComputeHash(user.Salt, password);

            if (user.PasswordHash == passwordHash)
            {
                Console.WriteLine("Login successful!");
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }

    private static string GenerateSalt()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes);
    }

    private static string ComputeHash(string salt, string password)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var combined = new byte[saltBytes.Length + passwordBytes.Length];
        Array.Copy(saltBytes, 0, combined, 0, saltBytes.Length);
        Array.Copy(passwordBytes, 0, combined, saltBytes.Length, passwordBytes.Length);

        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(combined);
            return Convert.ToBase64String(hash);
        }
    }
}
