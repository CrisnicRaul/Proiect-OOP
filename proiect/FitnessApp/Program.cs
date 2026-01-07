using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Program
{
    private static DataStorage dataStorage;
    private static User currentUser = null;
    private const string DataFile = "fitness_data.json";
    public static void Main(string[] args)
    {
        dataStorage = DataService.Load<DataStorage>(DataFile);

        if(dataStorage.Admins.Count == 0)
        {
            dataStorage.Admins.Add(new Admin ("admin","admin123"));
            SaveData();
        }

        while(true)
        {
            if(currentUser == null)
            {
                ShowLoginMenu();
            }
            else if (currentUser is Admin)
            {
                ShowAdminMenu();
            }
            else if (currentUser is Client)
            {
                ShowClientMenu();
            }
        }
    }

    private static void SaveData()
    {
        DataService.Save(DataFile, dataStorage);
    }

    private static void ShowLoginMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Fitness App ===");
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register (Client)");
        Console.WriteLine("3. Exit");
        Console.Write("Choose option: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Login();
                break;
            case "2":
                Register();
                break;
            case "3":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option!");
                Console.ReadKey();
                break;
        }
    }

    private static void Login()
    {
        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();


        var admin = dataStorage.Admins.FirstOrDefault(a => 
            a.Username == username && a.Password == password);
        if (admin != null)
        {
            currentUser = admin;
            Console.WriteLine("Logged in as Admin!");
            Console.ReadKey();
            return;
        }

        // Check clients
        var client = dataStorage.Clients.FirstOrDefault(c => 
            c.Username == username && c.Password == password);
        if (client != null)
        {
            currentUser = client;
            Console.WriteLine("Logged in as Client!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Invalid credentials!");
        Console.ReadKey();
    }

    private static void Register()
    {
        Console.Write("Username: ");
        var username = Console.ReadLine();

        // Check if username exists
        if (dataStorage.Admins.Any(a => a.Username == username) ||
            dataStorage.Clients.Any(c => c.Username == username))
        {
            Console.WriteLine("Username already exists!");
            Console.ReadKey();
            return;
        }

        Console.Write("Password: ");
        var password = Console.ReadLine();

        var newClient = new Client(username, password);
        dataStorage.Clients.Add(newClient);
        SaveData();

        Console.WriteLine("Registration successful! You can now login.");
        Console.ReadKey();
    }

    private static void ShowAdminMenu()
    {
        Console.Clear();
        Console.WriteLine($"=== Admin Menu (Logged in as: {currentUser.Username}) ===");
        Console.WriteLine("1. Manage Gyms");
        Console.WriteLine("2. Manage Subscription Types");
        Console.WriteLine("3. Manage Classes");
        Console.WriteLine("4. View Statistics");
        Console.WriteLine("5. Logout");
        Console.Write("Choose option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ManageGyms();
                break;
            case "2":
                // TODO: Implement subscription management
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "3":
                // TODO: Implement class management
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "4":
                // TODO: Implement statistics
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "5":
                currentUser = null;
                break;
            default:
                Console.WriteLine("Invalid option!");
                Console.ReadKey();
                break;
        }
    }

    private static void ManageGyms()
    {
        Console.Clear();
        Console.WriteLine("== Gym management menu. Options:");
        Console.WriteLine("1.Create new Gym");
        Console.WriteLine("2.View all Gyms");
        Console.WriteLine("3.Delete a Gym");
        Console.WriteLine("4. Go back");
        Console.Write("Choose option:");
        var choice = Console.ReadLine();
        
        switch(choice)
        {
            case "1":
                CreateGym();
                break;
            case "2":
                ViewGyms();
                break;
            case "3":
                DeleteGym();
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Invalid option!");
                Console.ReadKey();
                break;
        }
    }

    private static void CreateGym()
    {
        Console.Clear();
        Console.Write("Enter gym name: ");
        var name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Invalid name!");
            Console.ReadKey();
            return;
        }

        var gym = new Gym { Name = name };

        // Add zones
        while (true)
        {
            Console.Write("Add a zone? (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y")
                break;

            Console.Write("Zone name: ");
            var zoneName = Console.ReadLine();
            Console.Write("Zone capacity: ");
            if (int.TryParse(Console.ReadLine(), out int capacity))
            {
                gym.Zones.Add(new Zone { Name = zoneName, Capacity = capacity });
                Console.WriteLine("Zone added!");
            }
            else
            {
                Console.WriteLine("Invalid capacity!");
            }
        }

        dataStorage.Gyms.Add(gym);
        SaveData();
        Console.WriteLine("Gym created successfully!");
        Console.ReadKey();
    }

    private static void ViewGyms()
    {
        Console.Clear();
        Console.WriteLine("=== All Gyms ===");

        if (dataStorage.Gyms.Count == 0)
        {
            Console.WriteLine("No gyms available.");
        }
        else
        {
            for (int i = 0; i < dataStorage.Gyms.Count; i++)
            {
                var gym = dataStorage.Gyms[i];
                Console.WriteLine($"{i + 1}. {gym.Name}");
                if (gym.Zones.Count > 0)
                {
                    Console.WriteLine("   Zones:");
                    foreach (var zone in gym.Zones)
                    {
                        Console.WriteLine($"   - {zone.Name} (Capacity: {zone.Capacity})");
                    }
                }
            }
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void DeleteGym()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Gym ===");

        if (dataStorage.Gyms.Count == 0)
        {
            Console.WriteLine("No gyms to delete.");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < dataStorage.Gyms.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {dataStorage.Gyms[i].Name}");
        }

        Console.Write("Enter gym number to delete (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= dataStorage.Gyms.Count)
        {
            dataStorage.Gyms.RemoveAt(choice - 1);
            SaveData();
            Console.WriteLine("Gym deleted!");
        }
        else
        {
            Console.WriteLine("Cancelled or invalid choice.");
        }

        Console.ReadKey();
    }

    private static void ShowClientMenu()
    {
        Console.Clear();
        Console.WriteLine($"=== Client Menu (Logged in as: {currentUser.Username}) ===");
        Console.WriteLine("1. View Gyms");
        Console.WriteLine("2. Buy Subscription");
        Console.WriteLine("3. Book Class");
        Console.WriteLine("4. View My Subscriptions");
        Console.WriteLine("5. View My Reservations");
        Console.WriteLine("6. Cancel Reservation");
        Console.WriteLine("7. Logout");
        Console.Write("Choose option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ViewGyms();
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "2":
                // TODO: Implement buy subscription
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "3":
                // TODO: Implement book class
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "4":
                // TODO: Implement view subscriptions
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "5":
                // TODO: Implement view reservations
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "6":
                // TODO: Implement cancel reservation
                Console.WriteLine("Not implemented yet");
                Console.ReadKey();
                break;
            case "7":
                currentUser = null;
                break;
            default:
                Console.WriteLine("Invalid option!");
                Console.ReadKey();
                break;
        }
    }

}


public abstract class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    
    protected User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class Admin : User
{
    public Admin(string username, string password)
        : base(username, password) { }
}


public class Client : User
{
    public List<Subscription> Subscriptions { get; set; } = [];
    public List<Reservation> Reservations { get; set; } = [];

    public Client(string username, string password)
        : base(username, password) { }
}

public class Gym
{
    public string Name{get; set;}
    public List<Zone> Zones {get; set;} = [];
}

public class Zone
{
    public string Name { get; set; }
    public int Capacity { get; set; }
}

public class Subscription
{
    public string Type { get; set; }
    public double Price { get; set; }
    public DateTime StartDate { get; set; }
    public int DurationDays { get; set; }

    public bool IsActive =>
        DateTime.Now <= StartDate.AddDays(DurationDays);
}

public class FitnessClass
{
    public string Name { get; set; }
    public string Trainer { get; set; }
    public int Capacity { get; set; }
    public HashSet<string> ReservedUsers { get; set; } = [];
}

public class Reservation
{
    public string ClassName {get; set;}
}

static class DataService
{
    public static void Save<T>(string file, T data)
    {
        try
        {
            File.WriteAllText(file,
                JsonSerializer.Serialize(data, new JsonSerializerOptions
                { WriteIndented = true }));
        }
        catch (Exception e)
        {
            Console.WriteLine("Eroare salvare: " + e.Message);
        }
    }

    public static T Load<T>(string file) where T : new()
    {
        try
        {
            if (!File.Exists(file))
                return new T();

            return JsonSerializer.Deserialize<T>(
                File.ReadAllText(file)) ?? new T();
        }
        catch
        {
            Console.WriteLine("Fișier invalid. Se creează date noi.");
            return new T();
        }
    }
}

class DataStorage
{
    public List<Admin> Admins { get; set; } = [];
    public List<Client> Clients { get; set; } = [];
    public List<Gym> Gyms { get; set; } = [];
    public List<FitnessClass> Classes { get; set; } = [];
    public List<Subscription> SubscriptionTypes { get; set; } = [];

}