using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class IssueData
{
    //TO DO: asignee: user
    public int Id { get; set; }
    public string Name { get; set; }
    public string Release { get; set; }
    public string Sprint { get; set; }
    public IssueType Type { get; set; }

    public IssueData(string name)
    {
        Name = name;
    }
}
