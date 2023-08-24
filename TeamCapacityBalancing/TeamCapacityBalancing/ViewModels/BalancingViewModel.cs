using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class BalancingViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private SplitViewDisplayMode _mode = SplitViewDisplayMode.CompactInline;

    public List<IssueData> Epics { get; set; } = new() { new("Epic 1"), new("Epic 2"), new ("Epic 3") };

    public List<IssueData> Storyes { get; set; } = new() { new("Story 1"), new("Story 2"), new ("Story 3") };

    public List<string> Users { get; set; } = new() { "Ana", "Maria" };

}
