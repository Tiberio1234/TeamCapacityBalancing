using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using static System.Net.Mime.MediaTypeNames;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class InputViewModel : ObservableObject
{
    public ServiceCollection? _serviceCollection;
    public int hours=0;

    public InputViewModel()
    {

    }

    public InputViewModel(ServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    [RelayCommand]
    public void AtClose()
    {
        if (_serviceCollection != null)
        {
            var vm = _serviceCollection.GetService(typeof(TeamViewModel));
            if (vm != null)
            {
                ((TeamViewModel)vm).SelectedUserYourTeam.HoursPerDay = hours;
            }
        }
    }
}
