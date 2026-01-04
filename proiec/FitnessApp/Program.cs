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

