using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Npgsql.Internal.TypeMapping;
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
    private int currentEpicId = 0;
    private List<Tuple<User, float>> totalWork;

    //services
    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();
    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();

    [ObservableProperty]
    public List<User> _allUsers;
    [ObservableProperty]
    public List<OpenTasksUserAssociation> _openTasks;
    public ObservableCollection<IssueData> Epics { get; set; } = new ObservableCollection<IssueData>();
    public List<float> remaining = new();
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
    private bool _miniMessage = true;

    [ObservableProperty]
    private string _BusinessCaseString = string.Empty;

    [ObservableProperty]
    private bool _isShortTermVisible = false;

    [ObservableProperty]
    private bool _isBalancing = false;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    //[ObservableProperty]
    //private bool _sumsOpen = true;
    [ObservableProperty]
    private bool _ByPlaceHolder = false;

    [ObservableProperty]
    private bool _ByBusinessCase = false;


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
                MiniMessage = false;
                _selectedUser = value;

                if (value != null)
                {
                    currentEpicId = -1;
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
                    else
                    {
                        PopulateDefaultTeamUsers();
                    }
                    ShowAllStories();
                }

                foreach (var user in TeamMembers)
                {
                    if (OpenTasks.FirstOrDefault(x => x.User.Id == user.Id) == null)
                    {
                        OpenTasks.Add(new OpenTasksUserAssociation(user, 0));
                    }
                }
                while (OpenTasks.Count < MaxNumberOfUsers)
                {
                    OpenTasks.Add(new OpenTasksUserAssociation(new User
                    {
                        Username = "default"
                    }, 0));
                }

                OrderTeamAndStoryInfo();

                //CalculateWork();
                //OrderTeamAndStoryInfo();
                //InitializeTotals();
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
                 new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                MaxNumberOfUsers
            ),
    };


    public DateTime finishDate;


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
        CalculateCoverage();
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
        List<User> aux = new List<User>();
        for (int i = 0; i < MaxNumberOfUsers; i++)
        {
            User newUser = new User("default}");
            newUser.HasTeam = false;
            aux.Add(newUser);
        }
        TeamMembers = new ObservableCollection<User>(aux);
    }

    private List<Tuple<User, float>> GenerateDefaultDays()
    {
        List<Tuple<User, float>> capacityList = new();
        for (int i = 0; i < MaxNumberOfUsers; i++)
        {
            capacityList.Add(Tuple.Create(new User { Username = TeamMembers[i].Username, HasTeam = false }, 0f));
        }
        return capacityList;
    }

    private void PopulateByDefault()
    {
        List<Tuple<User, float>> capacityList = GenerateDefaultDays();

        foreach (IssueData story in allStories)
        {

            allUserStoryAssociation.Add(new UserStoryAssociation(story, false, story.Remaining, capacityList, MaxNumberOfUsers));
            MyUserAssociation.Add(allUserStoryAssociation.Last());
        }

    }

    private void ChangeColorByNumberOfDays()
    {
        for (int dayIndex = 0; dayIndex < Balancing[0].Days.Count; dayIndex++)
        {
            if (Balancing[0].Days[dayIndex].Value < 0)
                Balancing[0].ColorBackgroundBalancingList[dayIndex] = new SolidColorBrush(Colors.LightCoral);
            else if (Balancing[0].Days[dayIndex].Value < 4)
                Balancing[0].ColorBackgroundBalancingList[dayIndex] = new SolidColorBrush(Colors.LightYellow);
            else
                Balancing[0].ColorBackgroundBalancingList[dayIndex] = new SolidColorBrush(Colors.LightGreen);

        }
    }

    [RelayCommand]
    public void FilterByPlaceHolder()
    {
        if (ByPlaceHolder)
        {
            for (int userStoryAssociationIndex = 0; userStoryAssociationIndex < MyUserAssociation.Count; ++userStoryAssociationIndex)
            {
                if (!MyUserAssociation[userStoryAssociationIndex].StoryData.Name.Contains("#"))
                {
                    MyUserAssociation.Remove(MyUserAssociation[userStoryAssociationIndex]);
                    userStoryAssociationIndex--;
                }
            }
        }
        else
        {
            
            MyUserAssociation.Clear();
            foreach (UserStoryAssociation userStoryAssociation in allUserStoryAssociation)
            {
                MyUserAssociation.Add(userStoryAssociation);
            }

            if(currentEpicId != -1)
            {
                for (int userStoryAssociationIndex = 0; userStoryAssociationIndex < MyUserAssociation.Count; ++userStoryAssociationIndex)
                {
                    if (MyUserAssociation[userStoryAssociationIndex].StoryData.EpicID != currentEpicId)
                    {
                        MyUserAssociation.Remove(MyUserAssociation[userStoryAssociationIndex]);
                        userStoryAssociationIndex--;
                    }
                }
            }
        }
    }

    [RelayCommand]
    public void FilterByBusinessCase(string businessCase)
    {
        if (ByBusinessCase)
        {
            for(int issueIndex=0; issueIndex < Epics.Count; issueIndex++)
            {
                if (Epics[issueIndex].BusinessCase != businessCase)
                {
                   
                    if(MyUserAssociation.First().StoryData.EpicID == Epics[issueIndex].Id && MyUserAssociation.Count != allUserStoryAssociation.Count)
                    MyUserAssociation.Clear();
                
                    Epics.Remove(Epics[issueIndex]);
                    issueIndex--;
                }
            }
            if (MyUserAssociation.Count == allUserStoryAssociation.Count)
            {
                for (int userStoryAssociationIndex = 0; userStoryAssociationIndex < MyUserAssociation.Count; ++userStoryAssociationIndex)
                {
                    int issueIndex;
                    for (issueIndex = 0; issueIndex < Epics.Count; issueIndex++)
                    {
                        if (MyUserAssociation[userStoryAssociationIndex].StoryData.EpicID == Epics[issueIndex].Id)
                            break;
                    }
                    if (issueIndex == Epics.Count)
                        MyUserAssociation.Remove(MyUserAssociation[userStoryAssociationIndex]);

                }
            }
        }
        else
        {
            Epics.Clear();
            List<IssueData> epics = _queriesForDataBase.GetAllEpicsByTeamLeader(SelectedUser);
            foreach (var item in epics)
            {
                Epics.Add(item);
            }
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
        if (SelectedUser != null)
        {
            _navigationService!.CurrentPageType = typeof(SprintSelectionPage);
        }
    }

    [RelayCommand]
    public void SerializeOnSave()
    {
        if (SelectedUser == null)
        { return; }

        SerializeStoryData();


        var mainWindow = _serviceCollection.GetService(typeof(Window));
        var dialog = new SaveSuccessfulWindow("Saved succesfully!");
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog((MainWindow)mainWindow);
    }

    private void SerializeStoryData()
    {
        List<UserStoryDataSerialization> userStoryDataSerializations = new List<UserStoryDataSerialization>();

            for (int j = 0; j < allUserStoryAssociation.Count; j++)
            {
                List<Tuple<User, float>> capacityList = new List<Tuple<User, float>>();
                for (int i = 0; i < MaxNumberOfUsers; i++)
                {
                    capacityList.Add(new Tuple<User, float>(TeamMembers[i], allUserStoryAssociation[j].Days[i].Value));
                }
                userStoryDataSerializations.Add(new UserStoryDataSerialization(allUserStoryAssociation[j].StoryData, allUserStoryAssociation[j].ShortTerm, allUserStoryAssociation[j].Remaining, capacityList));
            }

        _jsonSerialization.SerializeUserStoryData(userStoryDataSerializations, SelectedUser.Username);
    }

    private void OrderTeamAndStoryInfo()
    {
        TeamMembers = new ObservableCollection<User>(TeamMembers.OrderBy(m => m.Username));
        foreach (UserStoryAssociation userStoryAssociation in MyUserAssociation)
        {
            userStoryAssociation.Days = new ObservableCollection<Wrapper<float>>(userStoryAssociation.Days.OrderBy(m => m.UserName));
        }
        OpenTasks = OpenTasks.OrderBy(x => x.User.Username).ToList();

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

        SerializeStoryData();

        CalculateCoverage();
    }

    private void SyncronizeDisplayedAsocListWithAllStoriesList()
    {
        for (int myUserAsocIndex = 0; myUserAsocIndex < MyUserAssociation.Count; myUserAsocIndex++)
        {
            for (int allUserAsocIndex = 0; allUserAsocIndex < allUserStoryAssociation.Count; allUserAsocIndex++)
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
            if (allUserStoryAssociation[allUserStoryAssociationIndex].StoryData.EpicID == epicId)
                MyUserAssociation.Add(allUserStoryAssociation[allUserStoryAssociationIndex]);
        }
    }

    public static double GetBusinessDays(DateTime startD, DateTime endD)
    {
        double calcBusinessDays =
            1 + ((endD - startD).TotalDays * 5 -
            (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

        if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
        if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;
        return calcBusinessDays;
    }

    public void CalculateTotalWorkOpenStory()
    {

    }

    [RelayCommand]
    public void EpicClicked(int id)
    {

        //SyncronizeDisplayedAsocListWithAllStoriesList();
        DisplayStoriesFromAnEpic(id);
        ShowShortTermStoryes();
        //get stories with same epicID and display them

        currentEpicId = id;
        FinalBalancing = true;
        GetStories = true;

        if (BusinessCaseString != string.Empty)
        { FilterByBusinessCase(BusinessCaseString); }

        FilterByPlaceHolder();

        CalculateCoverage();
        OrderTeamAndStoryInfo();
    }

    [RelayCommand]
    public void AllEpicsClicked()
    {
        allUserStoryAssociation.Clear();
        ShowAllStories();
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
        ShowShortTermStoryes();
        FinalBalancing = true;
        GetStories = true;
        currentEpicId = -1;

        if(BusinessCaseString != string.Empty) 
        { FilterByBusinessCase(BusinessCaseString); }
        
        FilterByPlaceHolder();
    }

    public void RefreshStories()
    {
        List<Tuple<User, float>> capacityList = GenerateDefaultDays();

        foreach (var story in allStories)
        {
            if (allUserStoryAssociation.FirstOrDefault(u => u.StoryData.Id == story.Id) == null)
            {
                allUserStoryAssociation.Add(new UserStoryAssociation(story, false, story.Remaining, capacityList, MaxNumberOfUsers));
                MyUserAssociation.Add(allUserStoryAssociation.Last());
            }
        }
    }

    [RelayCommand]
    public void RefreshClicked()
    {
        if (SelectedUser == null)
        {
            return;
        }
        MyUserAssociation.Clear();

        allUserStoryAssociation.Clear();

        allStories = _queriesForDataBase.GetAllStoriesByTeamLeader(SelectedUser);

        ShowAllStories();

        RefreshStories();

        OrderTeamAndStoryInfo();
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
    public void CalculateTotals()
    {
        var vm = _serviceCollection.GetService(typeof(SprintSelectionViewModel));
        if (vm != null)
        {
            if (((SprintSelectionViewModel)vm).Sprints.Count == 0)
            {
                IsBalancing = false;
                return;
            }
        }
        
        CalculateWork();
        OrderTeamAndStoryInfo();
        CalculateBalancing();
        InitializeTotals();
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
        if (SelectedUser == null)
        {
            return;
        }
        File.Delete(JsonSerialization.UserFilePath + SelectedUser!.Username);
        File.Delete(JsonSerialization.UserStoryFilePath + SelectedUser.Username);
        File.Delete(JsonSerialization.SprintPath + SelectedUser.Username);

        var mainWindow = _serviceCollection.GetService(typeof(Window));
        var dialog = new SaveSuccessfulWindow("Local files have been deleted successfully");
        dialog.Title = "Delete Local Files";
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog((MainWindow)mainWindow);

        PopulateDefaultTeamUsers();

        RefreshClicked();

    }

    [RelayCommand]
    public void OpenReleaseCalendar()
    {
        if (SelectedUser != null)
        {
            var vm = _serviceCollection.GetService(typeof(ReleaseCalendarViewModel));
            if (vm != null)
            {
                ((ReleaseCalendarViewModel)vm).GetSprintsFromSprintSelection();
            }

            _navigationService.CurrentPageType = typeof(ReleaseCalendarPage);
        }
    }
    public List<Tuple<User,float>> CalculateBalancing()
    {
        List<Tuple<User,float>> balance= new List<Tuple<User,float>>();
        var work = CalculateWork();
        var totalWork = GetTotalWork();
        for(int i=0;i<work.Count;i++) 
        {
            balance.Add(Tuple.Create(work[i].Item1, (float)Math.Round((work[i].Item2 - totalWork[i].Item2),2)));
        }
        Balancing[0] = new UserStoryAssociation(
                 new IssueData("Balancing", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                 true,
                 3.0f,
                 balance,
                 MaxNumberOfUsers
             );
        ChangeColorByNumberOfDays();
        return balance;
    }
    public List<Tuple<User, float>> CalculateOpenTasks()
    {
        List<Tuple<User, float>> workOpenStory = new List<Tuple<User, float>>();
        foreach (var item in OpenTasks)
        {
            workOpenStory.Add(Tuple.Create(item.User, (float)Math.Round(item.Remaining,2)));
        }
        workOpenStory.OrderBy(x=>x.Item1.Username).ToList();
        return workOpenStory;
    }
    public List<Tuple<User, float>> CalculateWorkOpenstories()
    {
        List<Tuple<User, float>> openstories = new List<Tuple<User, float>>();
        foreach (var member in TeamMembers)
        {
            float totalSum = 0;
            if(member.HasTeam)
            foreach (var item in allUserStoryAssociation)
            {
                foreach(var day in item.Days)
                if (member.Username == day.UserName)
                {
                    totalSum += day.Value * item.Remaining; 
                }
            }
            openstories.Add(Tuple.Create(member,(float)Math.Round(totalSum/100,2)));
        }

        return openstories;
    }
    public List<Tuple<User,float>> GetTotalWork()
    {
        List<Tuple<User,float>> totalWork= new List<Tuple<User,float>>();
        List<Tuple<User, float>> work = CalculateWorkOpenstories();
        List<Tuple<User, float>> openStories = CalculateOpenTasks();
        for (int i=0;i<work.Count;i++)
        {
            totalWork.Add(Tuple.Create(openStories[i].Item1,(float)Math.Round(work[i].Item2 + openStories[i].Item2,2)));
        }
        return totalWork;
        
    }
    public List<Tuple<User, float>> CalculateWork()
    {

        totalWork = new();

        int numberOfWorkingDays = 0;

        var vm = _serviceCollection.GetService(typeof(SprintSelectionViewModel));
        if (vm != null)
        {
            numberOfWorkingDays = ((SprintSelectionViewModel)vm).RemainingDays();
        }
        foreach (var item in TeamMembers)
        {
            totalWork.Add(Tuple.Create(item, (float)(numberOfWorkingDays)));
        }
        totalWork = totalWork.OrderBy(x => x.Item1.Username).ToList();
        return totalWork;
    }
    
    private void InitializeTotals()
    {

        totalWork = new List<Tuple<User, float>>(MaxNumberOfUsers);
        UserStoryAssociation a = new UserStoryAssociation(
              new IssueData("Total work open story", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
              true,
              3.0f,
              CalculateWorkOpenstories(),
              MaxNumberOfUsers
          );
        Totals[0] = a;
        Totals[1] = new UserStoryAssociation(
                  new IssueData("Total work", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                 true,
                 3.0f,
                 //we need a float list 
                 GetTotalWork(),
                 MaxNumberOfUsers
             ) ;
        Totals[2] = new UserStoryAssociation(
                 new IssueData("Total capacity", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                 true,
                 3.0f,
                 CalculateWork(),
                 MaxNumberOfUsers
             );
    }
    [ObservableProperty]
    private ObservableCollection<User> _teamMembers;
    public ObservableCollection<UserStoryAssociation> Totals { get; set; } = new ObservableCollection<UserStoryAssociation>()
    {
        new UserStoryAssociation(
                 new IssueData("Total work open story", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                 true,
                 3.0f,
                 new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                 MaxNumberOfUsers
             ),
          new UserStoryAssociation(
                 new IssueData("Total capacity", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                 true,
                 3.0f,
                 new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                 MaxNumberOfUsers
             ),
          new UserStoryAssociation(
                 new IssueData("Total capacity", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                 true,
                 3.0f,
                 new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                 MaxNumberOfUsers
             ),
    };
};




