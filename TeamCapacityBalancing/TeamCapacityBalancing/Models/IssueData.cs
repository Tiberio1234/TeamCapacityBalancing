namespace TeamCapacityBalancing.Models;

public class IssueData
{
    public enum IssueType
    {
        Epic,
        Story,
        Task,
    }

    public int? EpicID { get; set; }
    public int Id { get; set; }
    public string? Asignee { get; set; }
    public float Remaining { get; set; }
    public string? Name { get; set; }
    public string? Release { get; set; }
    public string? Sprint { get; set; }
    public bool Status { get; set; }
    public IssueType Type { get; set; }

    public IssueData()
    {

    }

    public IssueData(int id, string name)
    {
        Id = id;
        Name = name;
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

    public IssueData(int id, string summary, string assignee)
    {
        Id = id;
        Name = summary;
        Asignee = assignee; 
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

    public IssueData(string name)
    {
        Name = name;
    }
}
