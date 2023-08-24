using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.ViewModels;

enum DaysPerWeek
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
}

public partial class ResourceViewModel : ObservableObject
{
    private List<string> Days { get; set; } = new List<string>()
    {
      "Monday",
      "Tuesday",
      "Wednesday",
      "Thursday",
      "Friday"
    };


}
