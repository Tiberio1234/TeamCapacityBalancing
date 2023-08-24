using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class Epic : IssueData
{
    public Epic(string name) 
    {
        Name = name;
    }
}
