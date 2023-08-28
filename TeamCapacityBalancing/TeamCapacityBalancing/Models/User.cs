using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public partial class User
{
    public string Name { get; set;}
    public string UserName { get; set;}
    
    public bool HasTeam { get; set; }
    public User(string name) 
    { 
        Name= name;
    }
}
