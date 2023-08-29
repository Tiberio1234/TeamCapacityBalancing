using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.Services.ServicesAbstractions;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class TeamViewModel:ObservableObject
{
    private readonly PageService _pageService;
    private readonly NavigationService _navigationService;
    private string teamLeaderName = "teamLeader";   //injected in constructor

    //Services
    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();
    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();
    public List<PageData> Pages { get; }

    public TeamViewModel()
    {
        AllUser = _queriesForDataBase.GetAllUsers();
        YourTeam = _jsonSerialization.DeserializeTeamData(teamLeaderName);
    }

    public TeamViewModel(PageService pageService, NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
        Pages = _pageService.Pages.Select(x => x.Value).Where(x => x.ViewModelType != this.GetType()).ToList();
        AllUser = _queriesForDataBase.GetAllUsers();
        YourTeam = _jsonSerialization.DeserializeTeamData(teamLeaderName);
    }

    [ObservableProperty]
    private List<User> _allUser = new();

    [ObservableProperty]
    public List<User> _yourTeam;

    [RelayCommand]
    public void CreateTeam() 
    {   
        YourTeam = new();
       
        for (int i = 0; i < AllUser.Count; i++) 
        {
            if (AllUser[i].HasTeam) 
            {
                if (!YourTeam.Contains(AllUser[i]))
                {
                    YourTeam.Add(AllUser[i]);
                }
            }
        }

        
        _jsonSerialization.SerializeTeamData(YourTeam,teamLeaderName);
        

    }

    [RelayCommand]
    public void BackToPage() 
    {
        _navigationService.Team = YourTeam;
        _navigationService.CurrentPageType = typeof(BalancingPage);
     
    }

 
}
