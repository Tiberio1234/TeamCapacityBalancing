using Avalonia.Controls;
using Avalonia.Media;
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
    private List<IssueData> allStories = new List<IssueData>();
    private List<UserStoryAssociation> allUserStoryAssociation = new List<UserStoryAssociation>();

    //services
    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();
    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();

    [ObservableProperty]
    public List<User> _allUsers;
    [ObservableProperty]
    public List<OpenTasksUserAssociation> _openTasks;
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
        OpenTasks = _queriesForDataBase.GetRemainingForUser();
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
    private bool _getStories = false;

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
                    _getStories = false;
                    IsShortTermVisible = false;
                    FinalBalancing = false;
                    MyUserAssociation.Clear();
                    allUserStoryAssociation.Clear();
                    List<IssueData> epics;
                    epics = _queriesForDataBase.GetAllEpicsByTeamLeader(SelectedUser);
                    allStories = _queriesForDataBase.GetAllStoriesByTeamLeader(SelectedUser);
                    if (epics != null)
                    {
                        Epics = new ObservableCollection<IssueData>(epics);
                        OnPropertyChanged("Epics");
                    }
                    if (File.Exists(JsonSerialization.UserFilePath + SelectedUser.Username))
                    {
                        GetTeamUsers();
                    }
                    ShowAllStories();
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
                new List<float> { 10, -5, 0, 0, -5,  0, 0, 0, 0,0 },
                MaxNumberOfUsers
            ),  
    };
    

    public DateOnly finishDate;

    public float TotalWorkInShortTerm { get; set; }

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

            allUserStoryAssociation.Add(new UserStoryAssociation(ser.Story, ser.ShortTerm, ser.Remaining, capacityList, MaxNumberOfUsers));
            MyUserAssociation.Add(allUserStoryAssociation.Last());
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

        foreach (IssueData story in allStories)
        {
            
            allUserStoryAssociation.Add(new UserStoryAssociation(story, false, story.Remaining, capacityList, MaxNumberOfUsers));
            MyUserAssociation.Add(allUserStoryAssociation.Last());
        }

    }

    private void ChangeColorByNumberOfDays()
    {
        for(int dayIndex = 0; dayIndex < Balancing[0].Days.Count; dayIndex++)
        {
            if (Balancing[0].Days[dayIndex].Value < 0)
                Balancing[0].ColorBackgroundBalancingList[dayIndex] = new SolidColorBrush(Colors.Red);
            else if (Balancing[0].Days[dayIndex].Value < 4)
                Balancing[0].ColorBackgroundBalancingList[dayIndex] = new SolidColorBrush(Colors.Yellow);
            else
                Balancing[0].ColorBackgroundBalancingList[dayIndex] = new SolidColorBrush(Colors.Green);

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
    public void OpenSprintSelectionPage() 
    {
       _navigationService!.CurrentPageType=typeof(SprintSelectionPage);
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

        var mainWindow = _serviceCollection.GetService(typeof(Window));
        var dialog = new SaveSuccessfulWindow("Saved succesfully!");
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog((MainWindow)mainWindow);
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

    private void SyncronizeDisplayedAsocListWithAllStoriesList()
    {
        for(int myUserAsocIndex = 0; myUserAsocIndex < MyUserAssociation.Count; myUserAsocIndex++)
        {
            for(int allUserAsocIndex = 0; allUserAsocIndex < allUserStoryAssociation.Count; allUserAsocIndex++)
            {
                if (allUserStoryAssociation[allUserAsocIndex].StoryData.Name == MyUserAssociation[myUserAsocIndex].StoryData.Name)
                {
                    allUserStoryAssociation[allUserAsocIndex] = MyUserAssociation[myUserAsocIndex];
                    break;
                }
            }
        }

    }

    private void DisplayStoriesFromAnEpic(int epicId)
    {
       MyUserAssociation.Clear();
       for (int allUserStoryAssociationIndex = 0; allUserStoryAssociationIndex < allUserStoryAssociation.Count; allUserStoryAssociationIndex++)
       {
            if(allUserStoryAssociation[allUserStoryAssociationIndex].StoryData.EpicID == epicId)
            MyUserAssociation.Add(allUserStoryAssociation[allUserStoryAssociationIndex]);
       }
    }

    [RelayCommand]
    public void EpicClicked(int id)
    {

        //SyncronizeDisplayedAsocListWithAllStoriesList();
        DisplayStoriesFromAnEpic(id);

        //get stories with same epicID and display them



        FinalBalancing = true;
        GetStories = true;
        
        CalculateCoverage();
        OrderTeamAndStoryInfo();
    }

    [RelayCommand]
    public void ShowAllStories()
    {
        MyUserAssociation.Clear();
        if (File.Exists(JsonSerialization.UserStoryFilePath + SelectedUser.Username))
        {
            GetSerializedData();
        }
        else
        {
            PopulateByDefault();
        }

        FinalBalancing = true;
        GetStories = true;
    }

    [RelayCommand]
    public void RefreshClicked()
    {
        MyUserAssociation.Clear();
        GetSerializedData();

        List<IssueData> storyList = new List<IssueData>();
        storyList = _queriesForDataBase.GetAllStoriesByTeamLeader(SelectedUser);

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
                    //MyUserAssociation.Add(new UserStoryAssociation(story, false, story.Remaining, capacityList, MaxNumberOfUsers));
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

        for (int i = 0; i < allUserStoryAssociation.Count; i++)
        {
            if (allUserStoryAssociation[i].ShortTerm)
            {
                ShortTermStoryes.Add(allUserStoryAssociation[i]);
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

    [RelayCommand]

    public void DeleteLocalFiles() 
    {
        File.Delete(JsonSerialization.UserFilePath + SelectedUser!.Username);
        File.Delete(JsonSerialization.UserStoryFilePath + SelectedUser.Username);

        var mainWindow = _serviceCollection.GetService(typeof(Window));
        var dialog = new SaveSuccessfulWindow("Local files have been deleted successfully");
        dialog.Title = "Delete Local Files";
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog((MainWindow)mainWindow);

    }

    public ObservableCollection<UserStoryAssociation> Totals { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
       //new UserStoryAssociation(
       //         new IssueData("Total work open story", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
       //         true,
       //         3.0f,
       //         new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
       //         MaxNumberOfUsers

       //     ),
       //new UserStoryAssociation(
       //         new IssueData("Total work", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
       //         true,
       //         3.0f,
       //         new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
       //         MaxNumberOfUsers
       //     ),
       //new UserStoryAssociation(
       //         new IssueData("Total capacity", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
       //         true,
       //         3.0f,
       //         new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
       //         MaxNumberOfUsers
       //     ),
    };

    [ObservableProperty]
    private ObservableCollection<User> _teamMembers;
};




