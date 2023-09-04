﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public partial class ReleaseCalendarViewModel : ObservableObject
{
    public ServiceCollection? _serviceCollection;
    private readonly NavigationService? _navigationService;

    public ObservableCollection<Sprint> Sprints {set; get;}

    public ReleaseCalendarViewModel()
    {
    }

    public ReleaseCalendarViewModel(ServiceCollection serviceCollection,NavigationService navigationService)
    {
        _serviceCollection = serviceCollection;
        _navigationService = navigationService;
        var vm = _serviceCollection.GetService(typeof(SprintSelectionViewModel));
        if (vm != null)
        {
            Sprints = ((SprintSelectionViewModel)vm).Sprints;
        }
    }

    [RelayCommand]
    public void OpenBalancigPage()
    {
        _navigationService!.CurrentPageType = typeof(BalancingPage);
    }
}