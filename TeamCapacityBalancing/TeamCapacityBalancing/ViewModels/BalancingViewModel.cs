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

    public ObservableCollection<Tuple<User, float>> TotalWorkOpenStory { set; get; } = new ObservableCollection<Tuple<User, float>>();

    public ObservableCollection<UserStoryAssociation> MyUserAssociation { get; set; } = new ObservableCollection<UserStoryAssociation>
            {
                new UserStoryAssociation(
                    new IssueData("Story 1", 8.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                    true, 5.0f, 0.6f,
                    new List<UserDays>
                    {
                        new UserDays(new User("User 1","u1",1), 1.0f),
                        new UserDays(new User("User 2", "u1", 1), 2.0f),
                        new UserDays(new User("User 3", "u1", 1), 3.0f)
                    }
                ),
                new UserStoryAssociation(
                    new IssueData("Story 2", 5.0f, "Release 2", "Sprint 2", true, IssueData.IssueType.Story),
                    false, 2.0f, 0.8f,
                    new List<UserDays>
                    {
                        new UserDays(new User("User 1","u1",1), 12.0f),
                        new UserDays(new User("User 2", "u1", 1), 22.0f),
                        new UserDays(new User("User 3", "u1", 1), 32.0f)                    }
                )
            };

    public ObservableCollection<UserStoryAssociation> Totals { get; set; } = new ObservableCollection<UserStoryAssociation>

                {
                new UserStoryAssociation(
                    new IssueData("Total work open stories"),
                    true, 5.0f, 0.6f,
                    new List<UserDays>
                    {
                        new UserDays(new User("User 1","u1",1), 1.0f),
                        new UserDays(new User("User 2", "u1", 1), 2.0f),
                        new UserDays(new User("User 3", "u1", 1), 3.0f)
                    }
                ),
                new UserStoryAssociation(
                    new IssueData("Total work"),
                    false, 2.0f, 0.8f,
                    new List<UserDays>
                    {
                        new UserDays(new User("User 1","u1",1), 12.0f),
                        new UserDays(new User("User 2", "u1", 1), 22.0f),
                        new UserDays(new User("User 3", "u1", 1), 32.0f)                    }
                ),
             new UserStoryAssociation(
                    new IssueData("Total Capacity"),
                    false, 2.0f, 0.8f,
                    new List<UserDays>
                    {
                        new UserDays(new User("User 1","u1",1), 12.0f),
                        new UserDays(new User("User 2", "u1", 1), 22.0f),
                        new UserDays(new User("User 3", "u1", 1), 32.0f)                    }
                )
            };

    public ObservableCollection<UserStoryAssociation> Balancing { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
        new UserStoryAssociation(
                    new IssueData("Balancing"),
                    false, 2.0f, 0.8f,
                    new List<UserDays>
                    {
                        new UserDays(new User("User 1","u1",1), 12.0f),
                        new UserDays(new User("User 2", "u1", 1), 22.0f),
                        new UserDays(new User("User 3", "u1", 1), 32.0f)                    }
                )
    };


    public ObservableCollection<IssueData> Epics { get; set; } = new() { new("Epic 1"), new("Epic 2"), new("Epic 3") };
    public ObservableCollection<IssueData> Storyes { get; set; } = new() { new("Story 1"), new("Story 2"), new("Story 3") };

    public Team TeamMembers { get; set; } = new Team(new List<User>
            {
                new User("user1", "User One", 1),
                new User("user2", "User Two", 2),
                new User("user3", "User Three", 3),
            });
}

