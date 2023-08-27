using System;

namespace TeamCapacityBalancing.Models;

public class User
{

    public string Username { get; set; }
    public string DisplayName { get; set; }
    public int Id { get; set; }

    public User(string username, string displayName, int id)
    {
        Username = username;
        DisplayName = displayName;
        Id = id;
    }
}
