using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.Services.ServicesAbstractions;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class BalancingViewModel : ObservableObject
{
    private readonly PageService _pageService;
    private readonly NavigationService _navigationService;
    private readonly ServiceCollection _serviceCollection;
    private const int MaxNumberOfUsers = 10;
    private int EpicId;

    //services
    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();
    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();


    [ObservableProperty]
    public List<User> _allUsers;
    public ObservableCollection<IssueData> Epics { get; set; } = new ObservableCollection<IssueData>();
    public BalancingViewModel()
    {
    }

    public BalancingViewModel(PageService pageService, NavigationService navigationService, ServiceCollection serviceCollection)
    {
        _pageService = pageService;
        _navigationService = navigationService;
        _serviceCollection = serviceCollection;
        PopulateDefaultTeamUsers();
        AllUsers = _queriesForDataBase.GetAllTeamLeaders();
    }

    [ObservableProperty]
    private bool _isShortTermVisible = false;

    [ObservableProperty]
    private bool _isBalancing = false;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private bool _sumsOpen = true;

    [ObservableProperty]
    private bool _finalBalancing = false;

    [ObservableProperty]
    private bool _IsEpicClicked = false;

    private User _selectedUser;
    public User SelectedUser
    {
        get { return _selectedUser; }
        set
        {
            if (_selectedUser != value)
            {
                _selectedUser = value;

                if (value != null)
                {
                    IsEpicClicked = false;
                    FinalBalancing = false;
                    MyUserAssociation.Clear();
                    List<IssueData> epics;
                    epics = _queriesForDataBase.GetAllEpicsByTeamLeader(SelectedUser);
                    if (epics != null)
                    {
                        Epics = new ObservableCollection<IssueData>(epics);
                        OnPropertyChanged("Epics");
                    }
                }
                OnPropertyChanged(nameof(SelectedUser));
            }
        }
    }

    [ObservableProperty]
    private SplitViewDisplayMode _mode = SplitViewDisplayMode.CompactInline;


    public ObservableCollection<UserStoryAssociation> MyUserAssociation { get; set; } = new ObservableCollection<UserStoryAssociation>();


    public ObservableCollection<UserStoryAssociation> Balancing { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
           new UserStoryAssociation(
                new IssueData("Balancing", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            ),
    };

    private void GetSerializedData()
    {
        List<UserStoryDataSerialization> userStoryDataSerializations = new();
        userStoryDataSerializations = _jsonSerialization.DeserializeUserStoryData(SelectedUser.Username);
        foreach (UserStoryDataSerialization ser in userStoryDataSerializations)
        {
            List<float> capacityList = new List<float>();
            for (int i = 0; i < ser.UsersCapacity.Count; i++)
            {
                capacityList.Add(ser.UsersCapacity[i].Item2);
            }
            MyUserAssociation.Add(new UserStoryAssociation(ser.Story, ser.ShortTerm, ser.Remaining, capacityList));
        }

    }

    private void GetTeamUsers()
    {
        var aux = new ObservableCollection<User>(_jsonSerialization.DeserializeTeamData(SelectedUser.Username));
        for (int i = aux.Count; i < MaxNumberOfUsers; i++)
        {
            User newUser = new User($"user {i}");
            newUser.HasTeam = false;
            aux.Add(newUser);
        }
        for (int i = 0; i < MaxNumberOfUsers; i++)
        {
            TeamMembers[i] = aux[i];
        }
    }

    private void PopulateDefaultTeamUsers()
    {
        TeamMembers = new ObservableCollection<User>();
        for (int i = 0; i < MaxNumberOfUsers; i++)
        {
            User newUser = new User($"user {i}");
            newUser.HasTeam = false;
            TeamMembers.Add(newUser);
        }
    }

    private void PopulateByDefault()
    {
        List<float> capacityList = new List<float>();
        for (int i = 0; i < MaxNumberOfUsers; i++)
        {
            capacityList.Add(0);
        }
        List<IssueData> stories = new List<IssueData>();
        stories = _queriesForDataBase.GetStoriesByEpic(EpicId);


        foreach (IssueData story in stories)
        {
            MyUserAssociation.Add(new UserStoryAssociation(story, false, story.Remaining, capacityList));
        }

    }
    [RelayCommand]
    public void OpenTeamPage()
    {
        if (SelectedUser != null)
        {
            var vm = _serviceCollection.GetService(typeof(TeamViewModel));
            if (vm != null)
            {
                ((TeamViewModel)vm).PopulateUsersLists(SelectedUser.Username);
            }
            _navigationService.CurrentPageType = typeof(TeamPage);
            RefreshPage();
        }
    }

    [RelayCommand]
    public void SerializeOnSave()
    {
        List<UserStoryDataSerialization> userStoryDataSerializations = new List<UserStoryDataSerialization>();


        for (int j = 0; j < MyUserAssociation.Count; j++)
        {
            List<Tuple<User, float>> capacityList = new List<Tuple<User, float>>();
            for (int i = 0; i < MaxNumberOfUsers; i++)
            {
                capacityList.Add(new Tuple<User, float>(TeamMembers[i], MyUserAssociation[j].Days[i].Value));
            }
            userStoryDataSerializations.Add(new UserStoryDataSerialization(MyUserAssociation[j].StoryData, MyUserAssociation[j].ShortTerm, MyUserAssociation[j].Remaining, capacityList));
        }

        _jsonSerialization.SerializeUserStoryData(userStoryDataSerializations, SelectedUser.Username);

        //TODO: popUpMessage for saving
    }

    public void RefreshPage()
    {
        FinalBalancing = false;
        IsEpicClicked = false;
        IsShortTermVisible = false;
        IsBalancing = false;
    }

    [RelayCommand]
    public void EpicClicked(int id)
    {
        EpicId = id;
        if (File.Exists(JsonSerialization.UserFilePath + SelectedUser.Username))
        {
            GetTeamUsers();
        }
        if (File.Exists(JsonSerialization.UserStoryFilePath + SelectedUser.Username))
        {
            GetSerializedData();
        }
        else
        {
            PopulateByDefault();
        }
        FinalBalancing = true;
        IsEpicClicked = true;

    }

    [RelayCommand]
    public void CalculateCoverage()
    {
        for (int i = 0; i < MyUserAssociation.Count; i++)
        {
            MyUserAssociation[i].CalculateCoverage();
        }
    }

    public ObservableCollection<UserStoryAssociation> Totals { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
       new UserStoryAssociation(
                new IssueData("Total work open story", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            ),
       new UserStoryAssociation(
                new IssueData("Total work", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            ),
       new UserStoryAssociation(
                new IssueData("Total capacity", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            ),
    };

    [ObservableProperty]
    private ObservableCollection<User> _teamMembers;
};




