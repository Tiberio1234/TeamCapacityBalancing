namespace TeamCapacityBalancing.Models;

public class User
{
    public string Username { get; set; }

    public string DisplayName { get; set; }

    public bool HasTeam { get; set; }

    public int Id { get; set; }

    public Wrapper<int> HoursPerDay { get; set; }

    public User()
    {
        Username = string.Empty;
        DisplayName = string.Empty;
        HasTeam = false;
        Id = 0;
        HoursPerDay = new Wrapper<int>() { Value = 0 };
    }

    public User(string username, string displayName, int id)
    {
        Username = username;
        DisplayName = displayName;
        Id = id;
        HasTeam = false;
        HoursPerDay = new Wrapper<int>() { Value = 0 };
    }

    public User(string username, string displayName, bool hasTeam, int id)
    {
        Username = username;
        DisplayName = displayName;
        HasTeam = hasTeam;
        Id = id;
        HoursPerDay = new Wrapper<int>() { Value = 0 };
    }

    public User(string username)
    {
        Username = username;
        HasTeam = false;
        Id = 0;
        DisplayName = string.Empty;
        HoursPerDay = new Wrapper<int>() { Value = 0 };
    }
}
