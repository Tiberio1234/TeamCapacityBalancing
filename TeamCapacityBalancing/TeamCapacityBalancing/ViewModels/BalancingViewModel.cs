using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class BalancingViewModel : ObservableObject
{
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
                new UserStoryAssociation(new IssueData("Story 1"), true, 5.0f, new UserAssociation(new User("user1", "User One", 1), new List<float> { 1.5f, 2.0f, 1.0f }, true), 80.0f),
                new UserStoryAssociation(new IssueData("Story 2"), false, 3.5f,new UserAssociation(new User("user2", "User Two", 2), new List < float > { 0.5f, 1.0f }, false), 60.0f),
                new UserStoryAssociation(new IssueData("Story 3"), true, 7.0f, new UserAssociation(new User("user3", "User Three", 3), new List < float > { 2.0f, 2.5f, 2.5f, 1.0f }, false), 90.0f),
                new UserStoryAssociation(new IssueData("Story 4"), false, 4.0f,new UserAssociation(new User("user4", "User Four", 4), new List < float > { 1.0f, 1.5f }, false), 40.0f),
                new UserStoryAssociation(new IssueData("Story 5"), true, 6.5f, new UserAssociation(new User("user5", "User Five", 5), new List < float > { 2.5f, 1.5f, 2.5f }, false), 70.0f),
                new UserStoryAssociation(new IssueData("Story 6"), true, 6.5f, new UserAssociation(new User("user5", "User Six", 5), new List < float > { 2.5f, 1.5f, 2.5f }, false), 70.0f),
                new UserStoryAssociation(new IssueData("Story 7"), true, 6.5f, new UserAssociation(new User("user5", "User Seven", 5), new List < float > { 2.5f, 1.5f, 2.5f }, false), 70.0f),
                new UserStoryAssociation(new IssueData("Story 8"), true, 6.5f, new UserAssociation(new User("user5", "User Eight", 5), new List < float > { 2.5f, 1.5f, 2.5f }, false), 70.0f),
                new UserStoryAssociation(new IssueData("Story 9"), true, 6.5f, new UserAssociation(new User("user5", "User Nine", 5), new List < float > { 2.5f, 1.5f, 2.5f }, false), 70.0f),
                new UserStoryAssociation(new IssueData("Story 10"), true, 6.5f, new UserAssociation(new User("user5", "User Ten", 5), new List < float > { 2.5f, 1.5f, 2.5f }, false), 70.0f),
            };

    public ObservableCollection<UserStoryAssociation> Totals { get; set; } = new ObservableCollection<UserStoryAssociation>
            {
                new UserStoryAssociation(new IssueData("Total work open story"), true, 5.0f, new UserAssociation(new User("user1", "User One", 1), new List<float> { 1.5f, 2.0f, 1.0f }, true), 80.0f),
                new UserStoryAssociation(new IssueData("Total work"), true, 5.0f, new UserAssociation(new User("user1", "User One", 1), new List<float> { 1.5f, 2.0f, 1.0f }, true), 80.0f),
                new UserStoryAssociation(new IssueData("Total capacity"), true, 5.0f, new UserAssociation(new User("user1", "User One", 1), new List<float> { 1.5f, 2.0f, 1.0f }, true), 80.0f),
            };

    public ObservableCollection<UserStoryAssociation> Balancing { get; set; } = new ObservableCollection<UserStoryAssociation>
            {
                new UserStoryAssociation(new IssueData("Balancing"), true, 5.0f, new UserAssociation(new User("user1", "User One", 1), new List<float> { 1.5f, 2.0f, 1.0f }, true), 80.0f),
            };


    public ObservableCollection<IssueData> Epics { get; set; } = new() { new("Epic 1"), new("Epic 2"), new("Epic 3") };
    public ObservableCollection<IssueData> Storyes { get; set; } = new() { new("Story 1"), new("Story 2"), new("Story 3") };

    public Team TeamMembers { get; set; } = new Team(new List<User>
            {
                new User("user1", "User One", 1),
                new User("user2", "User Two", 2),
                new User("user2.2", "User Two", 2),
                new User("user2.2", "User Two", 2),
                new User("user2.2", "User Two", 2),
                new User("user3", "User Three", 3)
            });
}