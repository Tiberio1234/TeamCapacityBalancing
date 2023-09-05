﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class SprintSelectionViewModel : ObservableObject
{
    private readonly NavigationService? _navigationService;
    private readonly ServiceCollection _serviceCollection;
    public SprintSelectionViewModel()
    {

    }
    public SprintSelectionViewModel(PageService pageService, NavigationService navigationService, ServiceCollection serviceCollection)
    {
        _navigationService = navigationService;
        _serviceCollection = serviceCollection;
    }
    [ObservableProperty]
    private int _isShortTermVisible = 0;

    [ObservableProperty]
    private int _numberOfSprints = 0;

    public ObservableCollection<Sprint> Sprints { get; set; } = new ObservableCollection<Sprint>()
    { };
    private int CalculcateWorkingDays(DateTime start, DateTime end)
    {
        int workingDays = 0;
       //check if the start and end are the same dates
       while(start.Date!=end.Date)
        {
            if(start.DayOfWeek!=DayOfWeek.Saturday && start.DayOfWeek!=DayOfWeek.Sunday)
            {
                workingDays++;
            }
            start=start.AddDays(1);
        }
        return workingDays;
    }
    public int RemainingDays()
    {
        DateTime today= DateTime.Now;
        DateTime lastDate = DateTime.Parse(Sprints[Sprints.Count - 1].EndDate);
        return CalculcateWorkingDays(today, lastDate);
    }
    [ObservableProperty]
    public DateTimeOffset? _finishDate;

    [ObservableProperty]
    public bool _selecteSprintForShortTerm = false;

    [RelayCommand]
    public void GenerateSprints()
    {
        Sprints.Clear();
        for (int i = 0; i < NumberOfSprints; i++)
        {
            Sprints.Add(new Sprint($"Sprint {i + 1}", 0, false));
        }
    }
  
    [RelayCommand]
    public void OpenBalancigPage()
    {
        float totalWeeks = 0;

        DateTime dueStart=DateTime.Now;
        while(dueStart.DayOfWeek!=DayOfWeek.Monday)
        {
            dueStart=dueStart.AddDays(-1);
        }
        for (int i = 0; i < Sprints.Count; i++) 
        {
            Sprints[i].StartDate = dueStart.ToString("dd/MM/yyyy");
            dueStart = dueStart.AddDays(Sprints[i].NumberOfWeeks*7);
            while (dueStart.DayOfWeek != DayOfWeek.Friday)
            {
                dueStart = dueStart.AddDays(-1);
            }
            Sprints[i].EndDate = dueStart.ToString("dd/MM/yyyy");
            dueStart = dueStart.AddDays(+3);
        }
        for (int i = 0; i < Sprints.Count; i++)
        {
            if (Sprints[i].IsInShortTerm)
            {
                totalWeeks = totalWeeks + Sprints[i].NumberOfWeeks;
            }
        }

        var vm = _serviceCollection.GetService(typeof(BalancingViewModel));
        if (SelecteSprintForShortTerm)
        {
            if (vm != null)
            {
                ((BalancingViewModel)vm).TotalWorkInShortTerm = totalWeeks * 5;
            }
        }
        else
        {
            if (vm != null && FinishDate is not null)
            {
                ((BalancingViewModel)vm).finishDate = (FinishDate.Value.Date);
            }
        }
        _navigationService!.CurrentPageType = typeof(BalancingPage);
    }

    [RelayCommand]
    public void UncheckSprint() 
    {
        if (!SelecteSprintForShortTerm) 
        {
        foreach(var item in Sprints) 
            {
                item.IsInShortTerm=false;
            }
        }
    }
}
