using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.Services.ServicesAbstractions;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class BalancingViewModel : ObservableObject
{
    private readonly PageService? _pageService;
    private readonly NavigationService? _navigationService;
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
        ShowShortTermStoryes();
        AllUsers = _queriesForDataBase.GetAllTeamLeaders();
    }

    [ObservableProperty]
    private bool _isShortTermVisible = false;

    [ObservableProperty]
    private bool _isBalancing = false;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    //[ObservableProperty]
    //private bool _sumsOpen = true;

    [ObservableProperty]
    private bool _finalBalancing = false;

    [ObservableProperty]
    private bool _IsEpicClicked = false;

    private User? _selectedUser;
    public User? SelectedUser
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
                    IsShortTermVisible = false;
                    FinalBalancing = false;
                    MyUserAssociation.Clear();
                    List<IssueData> epics;
                    epics = _queriesForDataBase.GetAllEpicsByTeamLeader(SelectedUser);
                    if (epics != null)
                    {
                        Epics = new ObservableCollection<IssueData>(epics);
                        OnPropertyChanged("Epics");
                    }
                    if (File.Exists(JsonSerialization.UserFilePath + SelectedUser.Username))
                    {
                        GetTeamUsers();
                    }
                }
                OnPropertyChanged(nameof(SelectedUser));
            }
        }
    }

    [ObservableProperty]
    private SplitViewDisplayMode _mode = SplitViewDisplayMode.CompactInline;


    public ObservableCollection<UserStoryAssociation> MyUserAssociation { get; set; } = new ObservableCollection<UserStoryAssociation>();

    [ObservableProperty]
    public ObservableCollection<UserStoryAssociation> _shortTermStoryes;

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
            List<Tuple<User, float>> capacityList = new List<Tuple<User, float>>();
            for (int i = 0; i < ser.UsersCapacity.Count; i++)
            {
                capacityList.Add(new(ser.UsersCapacity[i].Item1, ser.UsersCapacity[i].Item2));
            }
            MyUserAssociation.Add(new UserStoryAssociation(ser.Story, ser.ShortTerm, ser.Remaining, capacityList));
        }

    }

    private void GetTeamUsers()
    {
        var aux = new ObservableCollection<User>(_jsonSerialization.DeserializeTeamData(SelectedUser.Username));
        for (int i = aux.Count; i < MaxNumberOfUsers; i++)
        {
            User newUser = new User("default");
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
            User newUser = new User("default}");
            newUser.HasTeam = false;
            TeamMembers.Add(newUser);
        }
    }

    private void PopulateByDefault()
    {
        List<Tuple<User, float>> capacityList = new();
        for (int i = 0; i < MaxNumberOfUsers; i++)
        {
            capacityList.Add(Tuple.Create(new User { Username = TeamMembers[i].Username, HasTeam = false}, 0f));
        }
        List<IssueData> stories = _queriesForDataBase.GetStoriesByEpic(EpicId);

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

    private void OrderTeamAndStoryInfo()
    {
        TeamMembers = new ObservableCollection<User>(TeamMembers.OrderBy(m => m.Username));
        foreach (UserStoryAssociation userStoryAssociation in MyUserAssociation)
        {
            userStoryAssociation.Days = new ObservableCollection<Wrapper<float>>(userStoryAssociation.Days.OrderBy(m => m.UserName));
        }
    }

    public void CreateDefaultListWithDays
        (List<Wrapper<float>> defaultList)
    {
        defaultList.Clear();
        for (int i = 0; i < MaxNumberOfUsers; i++)
        {
            defaultList.Add(new Wrapper<float> { UserName = "default", Value = 0 });    
        }
    }

    public void SyncTeamWithBalancingPageData()
    {
        //FinalBalancing = false;
        //IsEpicClicked = false;
        //IsShortTermVisible = false;
        //IsBalancing = false;
        if (File.Exists(JsonSerialization.UserFilePath + SelectedUser.Username))
        {
            GetTeamUsers();
        }

        List<User> teamMembers = TeamMembers.Where(user => user.Username != "default").ToList();

        List<Wrapper<float>> defaultList = new();


        foreach (UserStoryAssociation userStoryAssociation in MyUserAssociation)
        {

            CreateDefaultListWithDays(defaultList);

            for (int i = 0; i < teamMembers.Count; i++)
            {
                var exists = userStoryAssociation.Days.FirstOrDefault(x => x.UserName == teamMembers[i].Username);
                if (exists != null)
                {
                    defaultList[i].UserName = exists.UserName;
                    defaultList[i].Value = exists.Value;
                }
                else
                {
                    defaultList[i].UserName = teamMembers[i].Username;
                    defaultList[i].Value = 0;
                }
            }
            userStoryAssociation.Days = new ObservableCollection<Wrapper<float>>(defaultList);
        }

        OrderTeamAndStoryInfo();

        SerializeOnSave();

        CalculateCoverage();
    }

    [RelayCommand]
    public void EpicClicked(int id)
    {
        MyUserAssociation.Clear();
        EpicId = id;
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
        ShowShortTermStoryes();
        CalculateCoverage();
        OrderTeamAndStoryInfo();
    }

    [RelayCommand]
    public void RefreshClicked()
    {
        MyUserAssociation.Clear();
        GetSerializedData();

        List<IssueData> storyList = new List<IssueData>();
        storyList = _queriesForDataBase.GetStoriesByEpic(EpicId);

        if (MyUserAssociation.Count != storyList.Count)
        {
            foreach (IssueData story in storyList)
            {
                int indexMyUserAssociation;
                for (indexMyUserAssociation = 0; indexMyUserAssociation < MyUserAssociation.Count; indexMyUserAssociation++)
                {
                    if (MyUserAssociation[indexMyUserAssociation].StoryData.Name == story.Name)
                    {
                        break;
                    }
                }

                List<float> capacityList = Enumerable.Repeat(0f, 10).ToList();
                if (indexMyUserAssociation == MyUserAssociation.Count)
                {
                    MyUserAssociation.Add(new UserStoryAssociation(story, false, story.Remaining, capacityList));
                }

            }
        }

    }

    [RelayCommand]
    public void CalculateCoverage()
    {
        for (int i = 0; i < MyUserAssociation.Count; i++)
        {
            MyUserAssociation[i].CalculateCoverage();
        }

    }

    [RelayCommand]
    public void ShowShortTermStoryes()
    {
        ShortTermStoryes = new();

        for (int i = 0; i < MyUserAssociation.Count; i++)
        {
            if (MyUserAssociation[i].ShortTerm)
            {
                ShortTermStoryes.Add(MyUserAssociation[i]);
            }
        }

    }

    [RelayCommand]
    public void UncheckShortTermStory(string id)
    {
        for (int i = 0; i < MyUserAssociation.Count; i++)
        {
            if (MyUserAssociation[i].StoryData.Name == id)
            {
                MyUserAssociation[i].ShortTerm = false;
                ShortTermStoryes.Remove(MyUserAssociation[i]);
            }
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




