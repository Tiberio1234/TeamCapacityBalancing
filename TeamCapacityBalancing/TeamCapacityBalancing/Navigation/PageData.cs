using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.Navigation;

public class PageData
{
    public required Type Type { get; init;}
    public required string Name { get; init;}
    public required Type ViewModelType { get; init;}

}
