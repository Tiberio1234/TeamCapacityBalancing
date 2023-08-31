using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.Services.ServicesAbstractions;
using TeamCapacityBalancing.Views;

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
    public User? User { get; set; }

    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();

    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();

    private readonly PageService? _pageService;

    private readonly NavigationService? _navigationService;
    public ResourceViewModel()
    {
    }

    public ResourceViewModel(PageService pageService, NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
    }

    public void SetUser(User user)
    {
        User = user;
    }

    [RelayCommand]
    public void OpenBalacingPage()
    {
        if (_navigationService != null)
            _navigationService.CurrentPageType = typeof(BalancingPage);
    }

    [RelayCommand]
    public void BackToTeamPage()
    {
        if (_navigationService != null)
            _navigationService.CurrentPageType = typeof(TeamPage);
    }

    public List<DaysPerWeek> DaysPerWeeks { get; } = new List<DaysPerWeek>((DaysPerWeek[])Enum.GetValues(typeof(DaysPerWeek)));
    //public List<DaysPerWeek> DaysPerWeeks { get; } = new List<DaysPerWeek>(Enum.GetValues(typeof(DaysPerWeek)) as DaysPerWeek[]);
}
