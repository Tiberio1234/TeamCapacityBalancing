using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class SprintSelectionViewModel : ObservableObject
{
    [ObservableProperty]
    private int _isShortTermVisible = 0;

    [ObservableProperty]
    private int _numberOfSprints = 0;

    public ObservableCollection<Sprint> Sprints { get; set; } = new ObservableCollection<Sprint>()
    {};

    [ObservableProperty]
    private DateTime _finishDate;

    [RelayCommand]
    public void GenerateSprints()
    {
        for(int i=0;i<NumberOfSprints;i++)
        {
            Sprints.Add(new Sprint($"Sprint {i+1}", 0, false));
        }
    }
}
