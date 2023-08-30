using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.Services.ServicesAbstractions;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class TeamViewModel : ObservableObject
{
    private ServiceCollection _serviceCollection;
    private readonly PageService _pageService;
    private readonly NavigationService _navigationService;
    private string teamLeaderName = "teamLeader";   //injected in constructor

    //Services
    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();
    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();
    public List<PageData> Pages { get; }

    public TeamViewModel()
    {
    }

    public TeamViewModel(ServiceCollection serviceCollection, PageService pageService, NavigationService navigationService)
    {
        _serviceCollection = serviceCollection;
        _pageService = pageService;
        _navigationService = navigationService;
        Pages = _pageService.Pages.Select(x => x.Value).Where(x => x.ViewModelType != this.GetType()).ToList();
        PopulateUsersLists();
    }

    private void PopulateUsersLists()
    {
        YourTeam = new ObservableCollection<User>(_jsonSerialization.DeserializeTeamData(teamLeaderName));
        AllUsers = new ObservableCollection<User>(_queriesForDataBase.GetAllUsers());
        foreach (var user in YourTeam)
        {
            var u = AllUsers.FirstOrDefault(u => u.Id == user.Id);
            if (u != null)
            {
                AllUsers.Remove(u);
            }
        }
        CheckButtonsVisibility();
    }

    [ObservableProperty]
    private bool _addAllFromTeamEnable;

    [ObservableProperty]
    private bool _resourcePageVisibility;

    [ObservableProperty]
    private bool _removeAllFromTeamEnable;

    [ObservableProperty]
    private bool _addToTeamEnabledVisibility;

    [ObservableProperty]
    private bool _removeFromTeamVisibility;

    [ObservableProperty]
    private ObservableCollection<User> _allUsers = new();

    [ObservableProperty]
    private ObservableCollection<User> _yourTeam;

    private User _selectedUser;
    public User SelectedUser
    {
        get { return _selectedUser; }
        set
        {
            if (value == null)
            {
                RemoveFromTeamVisibility = false;
                AddToTeamEnabledVisibility = false;
                ResourcePageVisibility = false;
            }
            else if(_selectedUser != value)
            {
                if (YourTeam.Contains(value))
                {
                    RemoveFromTeamVisibility = true;
                    AddToTeamEnabledVisibility = false;
                    ResourcePageVisibility = true;
                }
                else
                {
                    RemoveFromTeamVisibility = false;
                    AddToTeamEnabledVisibility = true;
                    ResourcePageVisibility = false;
                }
            }
            _selectedUser = value;
            OnPropertyChanged(nameof(SelectedUser));
        }
    }

    private void CheckButtonsVisibility()
    {
        if (YourTeam.Count() == 0)
        {
            RemoveAllFromTeamEnable = false;
        }
        else
        {
            RemoveAllFromTeamEnable = true;
        }
        if (AllUsers.Count() == 0)
        {
            AddAllFromTeamEnable = false;
        }
        else
        {
            AddAllFromTeamEnable = true;
        }
    }

    [RelayCommand]
    public void AddAllToTeam()
    {
        YourTeam = new ObservableCollection<User>(AllUsers.Concat(YourTeam));
        AllUsers = new();
        foreach (var user in YourTeam)
        {
            user.HasTeam = true;
        }
        CheckButtonsVisibility();
        _jsonSerialization.SerializeTeamData(YourTeam.ToList(), teamLeaderName);
    }

    [RelayCommand]
    public void RemoveAllFromTeam()
    {
        AllUsers = new ObservableCollection<User>(_queriesForDataBase.GetAllUsers());
        YourTeam = new();
        CheckButtonsVisibility();
        _jsonSerialization.SerializeTeamData(YourTeam.ToList(), teamLeaderName);
    }

    [RelayCommand]
    public void RemoveFromTeam()
    {
        var existingUser = YourTeam.FirstOrDefault(u => u.Id == SelectedUser.Id);
        if (existingUser != null)
        {
            AllUsers.Add(existingUser);
            existingUser.HasTeam = false;
            YourTeam.Remove(existingUser);
        }
        CheckButtonsVisibility();
        _jsonSerialization.SerializeTeamData(YourTeam.ToList(), teamLeaderName);
    }

    [RelayCommand]
    public void AddToTeam()
    {
        var existingUser = AllUsers.FirstOrDefault(u => u.Id == SelectedUser.Id);
        if (existingUser != null)
        {
            YourTeam.Add(existingUser);
            existingUser.HasTeam = true;
            AllUsers.Remove(existingUser);
        }
        CheckButtonsVisibility();
        _jsonSerialization.SerializeTeamData(YourTeam.ToList(), teamLeaderName);
    }

    [RelayCommand]
    public void ResourcePage()
    {
        var vm = _serviceCollection.GetService(typeof(ResourceViewModel));
        if (vm != null)
        {
            ((ResourceViewModel)vm).SetUser(SelectedUser);
        }
        _navigationService.CurrentPageType = typeof(ResourcePage);
    }

    [RelayCommand]
    public void BackToPage()
    {
        _navigationService.CurrentPageType = typeof(BalancingPage);

    }


}
