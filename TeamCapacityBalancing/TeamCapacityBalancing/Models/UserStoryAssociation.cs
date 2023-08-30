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

public class UserStoryAssociation : Utility
{
    public IssueData StoryData { get; set; }
    public bool ShortTerm { get; set; }
    public float Remaining { get; set; }

    private ObservableCollection<float> _days;
    public ObservableCollection<float> Days
    {
        get => _days;
        set
        {
            _days = value;
            NotifyPropertyChanged("Days");
        }
    }


    public float Coverage { get; set; }
    public UserStoryAssociation(IssueData storyData, bool shortTerm, float remaining, List<float> days)
    {
        StoryData = storyData;
        ShortTerm = shortTerm;
        Remaining = remaining;
        Days = new ObservableCollection<float>(days);
        Coverage = 0;
    }
}