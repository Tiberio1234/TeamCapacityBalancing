using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
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

public sealed partial class BalancingViewModel : ObservableObject
{
    private readonly PageService _pageService;
    private readonly NavigationService _navigationService;

    //services
    private readonly IDataProvider _queriesForDataBase = new QueriesForDataBase();
    private readonly IDataSerialization _jsonSerialization = new JsonSerialization();
    public List<PageData> Pages { get; }

 

    [ObservableProperty]
    public List<User> _allUsers;
    public ObservableCollection<IssueData> Epics { get; set; } = new ObservableCollection<IssueData>();
    

    public BalancingViewModel()
    {

    }

    public BalancingViewModel(PageService pageService, NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
        Pages = _pageService.Pages.Select(x => x.Value).Where(x => x.ViewModelType != this.GetType()).ToList();
    }

    [ObservableProperty]
    private bool _isShortTermVisible = true;

    [ObservableProperty]
    private bool _isBalancing = true;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private bool _sumsOpen = true;

    [ObservableProperty]
    private SplitViewDisplayMode _mode = SplitViewDisplayMode.CompactInline;

    public ObservableCollection<UserStoryAssociation> MyUserAssociation { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
           new UserStoryAssociation(
                new IssueData("Sample Story 1", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
            ),
            new UserStoryAssociation(
                new IssueData("Sample Story 2", 8.0f, "Release 2", "Sprint 2", true, IssueData.IssueType.Story),
                false,
                5.0f,
                new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
            ),
    };

    public ObservableCollection<UserStoryAssociation> Balancing { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
           new UserStoryAssociation(
                new IssueData("Balancing", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
            ),
    };

    [RelayCommand]
    public void OpenTeamPage()
    {
     
        _navigationService.CurrentPageType = typeof(TeamPage);
    }

    public ObservableCollection<UserStoryAssociation> Totals { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
       new UserStoryAssociation(
                new IssueData("Total work open story", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
            ),
       new UserStoryAssociation(
                new IssueData("Total work", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
            ),
       new UserStoryAssociation(
                new IssueData("Total capacity", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
            ),
    };

   

    public Team TeamMembers { get; set; } = new Team(new List<User>
            {
                new User("user1", "User One", 1),
                new User("user2", "User Two", 2),
                new User("user3", "User Three", 3),
                new User("user4", "User Four", 3),
                new User("user5", "User Five", 3),
                new User("user6", "User Six", 3),
                new User("user7", "User Seven", 3),
                new User("user8", "User Eight", 3),
                new User("user9", "User Nine", 3),
                new User("user10", "User Ten", 3),
            });
}
