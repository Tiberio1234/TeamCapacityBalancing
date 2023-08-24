using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class Epic 
{
    public int Id { get; init; }
    public string Name { get; init; }
    //public string Release { get; init; }
   // public string Sprint { get; init; }

    public Epic(string name) 
    {
        Name = name;
    }
}
