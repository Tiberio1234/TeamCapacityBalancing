using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.Services.ServicesAbstractions;

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
    public User User { get; set; }

    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();

    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();

    private readonly PageService _pageService;

    private readonly NavigationService _navigationService;
    public ResourceViewModel()
    {
    }

    public ResourceViewModel(PageService pageService, NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
    }

    public void GetUsersData(int id)
    {
        var aux = _queriesForDataBase.GetAllUsers().FirstOrDefault(u => u.Id == id);
        if (aux != null)
        {
            User = aux;
        }
        else
        {
            throw new Exception("User not found");
        }
    }
    public List<DaysPerWeek> DaysPerWeeks { get; } = new List<DaysPerWeek>(Enum.GetValues(typeof(DaysPerWeek)) as DaysPerWeek[]);
}
