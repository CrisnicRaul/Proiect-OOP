using System;
using System.Collections.Generic;
using System.Linq;

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
    public List<Subscription> Subscriptions { get; set; } = new();
    public List<Reservation> Reservations { get; set; } = new();

    public Client(string username, string password)
        : base(username, password) { }
}

public class Gym
{
    public string Name{get; set;}
    public List<Zone> Zones {get; set;} = new();
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
    public List<string> ReservedUsers { get; set; } = new();
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