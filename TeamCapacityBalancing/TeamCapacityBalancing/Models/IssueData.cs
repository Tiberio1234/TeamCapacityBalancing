namespace TeamCapacityBalancing.Models
{
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
      
        public string Name { get; set; }
      
        public IssueType Type { get; set; }
      
        public string Assignee { get; set; }
        
        public int IssueNumber { get; set; }

        public int IssueId { get; set; }

        public IssueData(int id, string name, IssueType type, string assignee, int issueNumber, int issueId)
        {
            Id = id;
            Name = name;
            Type = type;
            Assignee = assignee;
            IssueNumber = issueNumber;
            IssueId = issueId;
        }

        public IssueData(string name)
        {
            Name = name;
        }
    }
}
