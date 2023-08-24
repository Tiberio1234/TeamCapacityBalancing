using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.ViewModels;

public enum DaysPerWeek
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
}

public sealed partial class ResourceViewModel : ObservableObject
{

    public List<DaysPerWeek> DaysPerWeeks { get; } = new List<DaysPerWeek>(Enum.GetValues(typeof(DaysPerWeek)) as DaysPerWeek[]);
}
