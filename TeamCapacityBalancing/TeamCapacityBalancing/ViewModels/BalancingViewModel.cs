﻿using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class BalancingViewModel : ObservableObject
{
    private readonly PageService _pageService;
    private readonly NavigationService _navigationService;
    public List<PageData> Pages { get; }

    public List<User> Team { get; set; }
    public BalancingViewModel()
    {
        
    }

    public BalancingViewModel(PageService pageService,NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
        Pages = _pageService.Pages.Select(x => x.Value).Where(x => x.ViewModelType != this.GetType()).ToList();
        Team = new();
        Team = _navigationService.team;
       
    }

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private SplitViewDisplayMode _mode = SplitViewDisplayMode.CompactInline;

    public List<IssueData> Epics { get; set; } = new() { new IssueData("Epic 1"," "," ")};

    public List<IssueData> Storyes { get; set; } = new();

    

    [RelayCommand]
    public void OpenTeamPage()  
    {
        _navigationService.CurrentPageType = typeof(TeamPage);
    }
}
      