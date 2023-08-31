using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
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
    private string teamLeaderName = "teamLeader";   //injected in constructor
    private const int MaxNumberOfUsers = 10; 
    private int EpicId = 10100;
   



    //services
    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();
    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();
    public List<PageData> Pages { get; }



    [ObservableProperty]
    public List<User> _allUsers;
    public ObservableCollection<IssueData> Epics { get; set; } = new ObservableCollection<IssueData>();
    public BalancingViewModel()
    {
        GetTeamUsers();
        AllUsers = _queriesForDataBase.GetAllUsers();
    }

    public BalancingViewModel(PageService pageService, NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
        Pages = _pageService.Pages.Select(x => x.Value).Where(x => x.ViewModelType != this.GetType()).ToList();
        GetTeamUsers();

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
                List<IssueData> epics;
                epics = _queriesForDataBase.GetAllEpicsByTeamLeader(SelectedUser);
                if (epics != null)
                {
                    Epics = new ObservableCollection<IssueData>(epics);
                    OnPropertyChanged("Epics");
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
        userStoryDataSerializations = _jsonSerialization.DeserializeUserStoryData(teamLeaderName);
        foreach(UserStoryDataSerialization ser in  userStoryDataSerializations)
        {
            List<float> capacityList = new List<float>();
            for(int i =0; i<ser.UsersCapacity.Count; i++)
            {
                capacityList.Add(ser.UsersCapacity[i].Item2);
            }
            MyUserAssociation.Add(new UserStoryAssociation(ser.Story,ser.ShortTerm,ser.Remaining,capacityList));
        }

    }

    private void GetTeamUsers()
    {
        TeamMembers = _jsonSerialization.DeserializeTeamData(teamLeaderName);
        for(int i = TeamMembers.Count; i < MaxNumberOfUsers; i++)
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
        

        foreach(IssueData story in stories)
        {
            MyUserAssociation.Add(new UserStoryAssociation(story, false, story.Remaining, capacityList));
        }
        
    }
    [RelayCommand]
    public void OpenTeamPage()
    {
        _navigationService.CurrentPageType = typeof(TeamPage);
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
            
        _jsonSerialization.SerializeUserStoryData(userStoryDataSerializations, teamLeaderName);

        //TODO: popUpMessage for saving
    }

    [RelayCommand]
    public void EpicClicked()
    {
        GetTeamUsers();
        if (File.Exists(JsonSerialization.UserStoryFilePath + teamLeaderName))
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
        for(int i=0;i<MyUserAssociation.Count;i++)
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

        public List<User> TeamMembers { get; set; } = new List<User>();
    };


   

