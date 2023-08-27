using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class UserStoryAssociation
{
    public IssueData StoryData { get; set; }
    public bool ShortTerm { get; set; }
    public float Remaining { get; set; }
    public Tuple<User, List<float>> UserAndDays { get; set; }
    public float Coverage { get; set; }
    public UserStoryAssociation(IssueData storyData, bool shortTerm, float remaining, Tuple<User, List<float>> UserAndDays, float coverage)
    {
        StoryData = storyData;
        ShortTerm = shortTerm;
        Remaining = remaining;
        UserAndDays = UserAndDays;
        Coverage = coverage;
    }
}

//public IssueData StoryData { get; set; }
//public Tuple<IssueData, bool> ShortTerm { get; set; }
//public Tuple<IssueData, float> Remaining { get; set; }
//public Tuple<IssueData, float> Coverage { get; set; }

//public UserStoryAssociation(IssueData storyData, Tuple<IssueData, bool> shortTerm, Tuple<IssueData, float> remaining, Tuple<IssueData, float> coverage)
//{
//    StoryData = storyData;
//    ShortTerm = shortTerm;
//    Remaining = remaining;
//    Coverage = coverage;
//}

//public IssueData Story { get; set; }   
//public bool ShortTerm { get; set; }
//public List<Tuple<User, float>> UserAndDays { get; set; }
//public float Coverage { get; set; }