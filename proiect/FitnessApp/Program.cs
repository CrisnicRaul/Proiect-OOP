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
                ManageSubscriptionTypes();
                Console.ReadKey();
                break;
            case "3":
                ManageClasses();
                Console.ReadKey();
                break;
            case "4":
                ViewStastistics();
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

    private static void ManageSubscriptionTypes()
    {
        Console.Clear();
        Console.WriteLine("=== Subscription Type Management ===");
        Console.WriteLine("1. Create Subscription Type");
        Console.WriteLine("2. View All Subscription Types");
        Console.WriteLine("3. Delete Subscription Type");
        Console.WriteLine("4. Go Back");
        Console.Write("Choose option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                CreateSubscriptionType();
                break;
            case "2":
                ViewSubscriptionTypes();
                break;
            case "3":
                DeleteSubscriptionType();
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Invalid option!");
                Console.ReadKey();
                break;
        }
    }

    private static void CreateSubscriptionType()
    {
        Console.Clear();
        Console.WriteLine("=== Create Subscription Type ===");

        Console.Write("Enter subscription name (e.g., Monthly, Annual): ");
        var name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Invalid name!");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter price: ");
        if (!double.TryParse(Console.ReadLine(), out double price) || price <= 0)
        {
            Console.WriteLine("Invalid price!");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter duration in days: ");
        if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0)
        {
            Console.WriteLine("Invalid duration!");
            Console.ReadKey();
            return;
        }

        var subscriptionType = new Subscription
        {
            Type = name,
            Price = price,
            DurationDays = duration,
            StartDate = DateTime.MinValue // Template - not an active subscription
        };

        dataStorage.SubscriptionTypes.Add(subscriptionType);
        SaveData();
        Console.WriteLine("Subscription type created successfully!");
        Console.ReadKey();
    }

    private static void ViewSubscriptionTypes()
    {
        Console.Clear();
        Console.WriteLine("=== All Subscription Types ===");

        if (dataStorage.SubscriptionTypes.Count == 0)
        {
            Console.WriteLine("No subscription types available.");
        }
        else
        {
            for (int i = 0; i < dataStorage.SubscriptionTypes.Count; i++)
            {
                var sub = dataStorage.SubscriptionTypes[i];
                Console.WriteLine($"{i + 1}. {sub.Type} - ${sub.Price} for {sub.DurationDays} days");
            }
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void DeleteSubscriptionType()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Subscription Type ===");

        if (dataStorage.SubscriptionTypes.Count == 0)
        {
            Console.WriteLine("No subscription types to delete.");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < dataStorage.SubscriptionTypes.Count; i++)
        {
            var sub = dataStorage.SubscriptionTypes[i];
            Console.WriteLine($"{i + 1}. {sub.Type} - ${sub.Price}");
        }

        Console.Write("Enter number to delete (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= dataStorage.SubscriptionTypes.Count)
        {
            dataStorage.SubscriptionTypes.RemoveAt(choice - 1);
            SaveData();
            Console.WriteLine("Subscription type deleted!");
        }
        else
        {
            Console.WriteLine("Cancelled or invalid choice.");
        }

        Console.ReadKey();
    }

    private static void ManageClasses()
    {
        Console.Clear();
        Console.WriteLine("=== Class Management ===");
        Console.WriteLine("1. Create Class");
        Console.WriteLine("2. View All Classes");
        Console.WriteLine("3. Delete Class");
        Console.WriteLine("4. Go Back");
        Console.Write("Choose option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                CreateClass();
                break;
            case "2":
                ViewClasses();
                break;
            case "3":
                DeleteClass();
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Invalid option!");
                Console.ReadKey();
                break;
        }
    }

    private static void CreateClass()
    {
        Console.Clear();
        Console.WriteLine("=== Create Fitness Class ===");

        Console.Write("Enter class name: ");
        var name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Invalid name!");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter trainer name: ");
        var trainer = Console.ReadLine();

        Console.Write("Enter capacity: ");
        if (!int.TryParse(Console.ReadLine(), out int capacity) || capacity <= 0)
        {
            Console.WriteLine("Invalid capacity!");
            Console.ReadKey();
            return;
        }

        var fitnessClass = new FitnessClass
        {
            Name = name,
            Trainer = trainer,
            Capacity = capacity
        };

        dataStorage.Classes.Add(fitnessClass);
        SaveData();
        Console.WriteLine("Class created successfully!");
        Console.ReadKey();
    }

    private static void ViewClasses()
    {
        Console.Clear();
        Console.WriteLine("=== All Fitness Classes ===");

        if (dataStorage.Classes.Count == 0)
        {
            Console.WriteLine("No classes available.");
        }
        else
        {
            for (int i = 0; i < dataStorage.Classes.Count; i++)
            {
                var fc = dataStorage.Classes[i];
                int spotsLeft = fc.Capacity - fc.ReservedUsers.Count;
                Console.WriteLine($"{i + 1}. {fc.Name} - Trainer: {fc.Trainer} - Spots: {spotsLeft}/{fc.Capacity}");
            }
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void DeleteClass()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Fitness Class ===");

        if (dataStorage.Classes.Count == 0)
        {
            Console.WriteLine("No classes to delete.");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < dataStorage.Classes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {dataStorage.Classes[i].Name}");
        }

        Console.Write("Enter number to delete (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= dataStorage.Classes.Count)
        {
            var deletedClass = dataStorage.Classes[choice - 1];
            
            // Remove reservations for this class from all clients
            foreach (var client in dataStorage.Clients)
            {
                client.Reservations.RemoveAll(r => r.ClassName == deletedClass.Name);
            }
            
            dataStorage.Classes.RemoveAt(choice - 1);
            SaveData();
            Console.WriteLine("Class deleted!");
        }
        else
        {
            Console.WriteLine("Cancelled or invalid choice.");
        }

        Console.ReadKey();
    }

    private static void ViewStatistics()
    {
        Console.Clear();
        Console.WriteLine("=== Statistics ===\n");

        // General counts
        Console.WriteLine($"Total Clients: {dataStorage.Clients.Count}");
        Console.WriteLine($"Total Gyms: {dataStorage.Gyms.Count}");
        Console.WriteLine($"Total Classes: {dataStorage.Classes.Count}");
        Console.WriteLine($"Total Subscription Types: {dataStorage.SubscriptionTypes.Count}");

        // Active subscriptions
        int activeSubscriptions = dataStorage.Clients
            .SelectMany(c => c.Subscriptions)
            .Count(s => s.IsActive);
        Console.WriteLine($"\nActive Subscriptions: {activeSubscriptions}");

        // Total revenue
        double totalRevenue = dataStorage.Clients
            .SelectMany(c => c.Subscriptions)
            .Sum(s => s.Price);
        Console.WriteLine($"Total Revenue: ${totalRevenue:F2}");

        // Most popular class
        if (dataStorage.Classes.Count > 0)
        {
            var mostPopular = dataStorage.Classes
                .OrderByDescending(c => c.ReservedUsers.Count)
                .First();
            Console.WriteLine($"\nMost Popular Class: {mostPopular.Name} ({mostPopular.ReservedUsers.Count} reservations)");
        }

        // Subscription breakdown
        Console.WriteLine("\n--- Subscription Type Breakdown ---");
        foreach (var subType in dataStorage.SubscriptionTypes)
        {
            int count = dataStorage.Clients
                .SelectMany(c => c.Subscriptions)
                .Count(s => s.Type == subType.Type);
            Console.WriteLine($"{subType.Type}: {count} purchased");
        }

        // Class occupancy
        Console.WriteLine("\n--- Class Occupancy ---");
        foreach (var fc in dataStorage.Classes)
        {
            double occupancy = fc.Capacity > 0 
                ? (double)fc.ReservedUsers.Count / fc.Capacity * 100 
                : 0;
            Console.WriteLine($"{fc.Name}: {occupancy:F1}% ({fc.ReservedUsers.Count}/{fc.Capacity})");
        }

        Console.WriteLine("\nPress any key to continue...");
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
                Console.ReadKey();
                break;
            case "2":
                BuySubscription();
                Console.ReadKey();
                break;
            case "3":
                BookClass();
                Console.ReadKey();
                break;
            case "4":
                ViewMySubscriptions();
                Console.ReadKey();
                break;
            case "5":
                ViewMyReservations();
                Console.ReadKey();
                break;
            case "6":
                CancelReservation();
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

    private static void BuySubscription()
    {
        Console.Clear();
        Console.WriteLine("=== Buy Subscription ===");

        if (dataStorage.SubscriptionTypes.Count == 0)
        {
            Console.WriteLine("No subscription types available. Please contact admin.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Available Subscription Types:");
        for (int i = 0; i < dataStorage.SubscriptionTypes.Count; i++)
        {
            var sub = dataStorage.SubscriptionTypes[i];
            Console.WriteLine($"{i + 1}. {sub.Type} - ${sub.Price} for {sub.DurationDays} days");
        }

        Console.Write("\nEnter number to purchase (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= dataStorage.SubscriptionTypes.Count)
        {
            var template = dataStorage.SubscriptionTypes[choice - 1];
            
            var newSubscription = new Subscription
            {
                Type = template.Type,
                Price = template.Price,
                DurationDays = template.DurationDays,
                StartDate = DateTime.Now
            };

            var client = (Client)currentUser;
            client.Subscriptions.Add(newSubscription);
            SaveData();

            Console.WriteLine($"\nSuccessfully purchased {template.Type} subscription!");
            Console.WriteLine($"Valid until: {newSubscription.StartDate.AddDays(newSubscription.DurationDays):yyyy-MM-dd}");
        }
        else if (choice != 0)
        {
            Console.WriteLine("Invalid choice.");
        }

        Console.ReadKey();
    }

    private static void BookClass()
    {
        Console.Clear();
        Console.WriteLine("=== Book a Class ===");

        var client = (Client)currentUser;

        // Check if client has active subscription
        if (!client.Subscriptions.Any(s => s.IsActive))
        {
            Console.WriteLine("You need an active subscription to book classes!");
            Console.ReadKey();
            return;
        }

        if (dataStorage.Classes.Count == 0)
        {
            Console.WriteLine("No classes available.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Available Classes:");
        var availableClasses = new List<FitnessClass>();
        int index = 1;

        foreach (var fc in dataStorage.Classes)
        {
            int spotsLeft = fc.Capacity - fc.ReservedUsers.Count;
            bool alreadyBooked = fc.ReservedUsers.Contains(client.Username);
            
            string status = alreadyBooked ? "[ALREADY BOOKED]" : 
                           spotsLeft == 0 ? "[FULL]" : $"[{spotsLeft} spots left]";
            
            Console.WriteLine($"{index}. {fc.Name} - Trainer: {fc.Trainer} {status}");
            availableClasses.Add(fc);
            index++;
        }

        Console.Write("\nEnter class number to book (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= availableClasses.Count)
        {
            var selectedClass = availableClasses[choice - 1];

            if (selectedClass.ReservedUsers.Contains(client.Username))
            {
                Console.WriteLine("You have already booked this class!");
            }
            else if (selectedClass.ReservedUsers.Count >= selectedClass.Capacity)
            {
                Console.WriteLine("This class is full!");
            }
            else
            {
                selectedClass.ReservedUsers.Add(client.Username);
                client.Reservations.Add(new Reservation { ClassName = selectedClass.Name });
                SaveData();
                Console.WriteLine($"Successfully booked {selectedClass.Name}!");
            }
        }
        else if (choice != 0)
        {
            Console.WriteLine("Invalid choice.");
        }

        Console.ReadKey();
    }
    
    private static void ViewMySubscriptions()
    {
        Console.Clear();
        Console.WriteLine("=== My Subscriptions ===");

        var client = (Client)currentUser;

        if (client.Subscriptions.Count == 0)
        {
            Console.WriteLine("You have no subscriptions.");
        }
        else
        {
            for (int i = 0; i < client.Subscriptions.Count; i++)
            {
                var sub = client.Subscriptions[i];
                var endDate = sub.StartDate.AddDays(sub.DurationDays);
                string status = sub.IsActive ? "ACTIVE" : "EXPIRED";
                
                Console.WriteLine($"{i + 1}. {sub.Type}");
                Console.WriteLine($"   Price: ${sub.Price}");
                Console.WriteLine($"   Start: {sub.StartDate:yyyy-MM-dd}");
                Console.WriteLine($"   End: {endDate:yyyy-MM-dd}");
                Console.WriteLine($"   Status: [{status}]");
                Console.WriteLine();
            }
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static void ViewMyReservations()
    {
        Console.Clear();
        Console.WriteLine("=== My Reservations ===");

        var client = (Client)currentUser;

        if (client.Reservations.Count == 0)
        {
            Console.WriteLine("You have no reservations.");
        }
        else
        {
            for (int i = 0; i < client.Reservations.Count; i++)
            {
                var reservation = client.Reservations[i];
                var fitnessClass = dataStorage.Classes.FirstOrDefault(c => c.Name == reservation.ClassName);
                
                if (fitnessClass != null)
                {
                    Console.WriteLine($"{i + 1}. {reservation.ClassName} - Trainer: {fitnessClass.Trainer}");
                }
                else
                {
                    Console.WriteLine($"{i + 1}. {reservation.ClassName} [Class no longer exists]");
                }
            }
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void CancelReservation()
    {
        Console.Clear();
        Console.WriteLine("=== Cancel Reservation ===");

        var client = (Client)currentUser;

        if (client.Reservations.Count == 0)
        {
            Console.WriteLine("You have no reservations to cancel.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Your Reservations:");
        for (int i = 0; i < client.Reservations.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {client.Reservations[i].ClassName}");
        }

        Console.Write("\nEnter number to cancel (0 to go back): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= client.Reservations.Count)
        {
            var reservation = client.Reservations[choice - 1];
            
            // Remove from class's reserved users
            var fitnessClass = dataStorage.Classes.FirstOrDefault(c => c.Name == reservation.ClassName);
            if (fitnessClass != null)
            {
                fitnessClass.ReservedUsers.Remove(client.Username);
            }
            
            // Remove from client's reservations
            client.Reservations.RemoveAt(choice - 1);
            SaveData();
            
            Console.WriteLine("Reservation cancelled successfully!");
        }
        else if (choice != 0)
        {
            Console.WriteLine("Invalid choice.");
        }

        Console.ReadKey();
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