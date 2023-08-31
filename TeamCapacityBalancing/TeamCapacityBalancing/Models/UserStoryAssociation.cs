using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Services;

namespace TeamCapacityBalancing.Models;

public class Wrapper<T>:Utility
{
    public T? _value; 
    public T? Value 
    {
        get => _value;
        set
        {
            _value = value;
            NotifyPropertyChanged();
        }
    }
    public Wrapper()
    {
    }
}

public class UserStoryAssociation : Utility
{
    public IssueData StoryData { get; set; }
    public bool ShortTerm { get; set; }
    public float Remaining { get; set; }

    private ObservableCollection<Wrapper<float>> _days;
    public ObservableCollection<Wrapper<float>> Days
    {
        get => _days;
        set
        {
            _days = value;
            NotifyPropertyChanged();
        }
    }


    public Wrapper<float> Coverage { get; set; }
    public UserStoryAssociation(IssueData storyData, bool shortTerm, float remaining, List<float> days)
    {
        StoryData = storyData;
        ShortTerm = shortTerm;
        Remaining = remaining;
        _days = new(days.Select(x => new Wrapper<float>() { Value = x}));
        Coverage = new Wrapper<float>() { Value = 0 };
    }

    public void CalculateCoverage()
    {
        Coverage.Value = Days.Sum(x => x.Value);
    }
}
