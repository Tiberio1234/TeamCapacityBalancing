using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class Team
{
    public List<User> team;

    public Team(List<User> users) 
    {
       team= users;
    }
}
