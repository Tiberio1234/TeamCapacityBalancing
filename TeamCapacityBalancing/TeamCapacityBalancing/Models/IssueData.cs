namespace TeamCapacityBalancing.Models;

public class IssueData
{
    public enum IssueType
    {
        Epic,
        Story,
        Task,
    }

    public string? BusinessCase { get; set; }

    public int? EpicID { get; set; }
    public int Id { get; set; }
    public string? Asignee { get; set; }
    public float Remaining { get; set; }
    public string? Name { get; set; }
    public string? Release { get; set; }
    public string? Sprint { get; set; }
    public bool Status { get; set; }
    public IssueType Type { get; set; }

    public IssueData(string? businessCase, int? epicID, int id, string? asignee, float remaining, string? name, string? release, string? sprint, bool status, IssueType type)
    {
        BusinessCase = businessCase;
        EpicID = epicID;
        Id = id;
        Asignee = asignee;
        Remaining = remaining;
        Name = name;
        Release = release;
        Sprint = sprint;
        Status = status;
        Type = type;
    }

    public IssueData(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public IssueData(int id, string name, string businessCase = "DoesntHave")
    {
        Id = id;
        Name = name;
        BusinessCase = businessCase;
    }

    public IssueData(string name, float remaining, string release, string sprint, bool status, IssueType type)
    {
        Name = name;
        Remaining = remaining;
        Release = release;
        Sprint = sprint;
        Status = status;
        Type = type;
    }

    
    public IssueData(int id, string summary, string assignee, float remaining)
    {
        Id = id;
        Name = summary;
        Asignee = assignee;
        Remaining = remaining;
    }

    public IssueData(int id,int epicId, string summary, string assignee, float remaining)
    {
        Id = id;
        EpicID = epicId;
        Name = summary;
        Asignee = assignee;
        Remaining = remaining;
    }

    public IssueData()
    {

    }

}
