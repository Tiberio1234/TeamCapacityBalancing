namespace TeamCapacityBalancing.Models;

public class IssueData
{
    public enum IssueType
    {
        Epic,
        Story,
        Task,
    }
    //TO DO: asignee: user
    public int Id { get; set; }
    public float Remaining { get; set; }
    public string Name { get; set; }
    public string Release { get; set; }
    public string Sprint { get; set; }
    public bool Status { get; set; }
    public IssueType Type { get; set; }

    public IssueData(string name, float remaining, string release, string sprint, bool status, IssueType type)
    {
        Name = name;
        Remaining = remaining;
        Release = release;
        Sprint = sprint;
        Status = status;
        Type = type;
    }

    public IssueData(string name)
    {
        Name = name;
    }
}
