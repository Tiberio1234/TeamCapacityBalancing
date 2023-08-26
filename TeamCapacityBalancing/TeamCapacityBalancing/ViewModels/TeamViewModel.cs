using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class TeamViewModel:ObservableObject
{
    private readonly PageService _pageService;
    private readonly NavigationService _navigationService;
    public List<PageData> Pages { get; }

    public TeamViewModel()
    {
        
    }

    public TeamViewModel(PageService pageService, NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
        Pages = _pageService.Pages.Select(x => x.Value).Where(x => x.ViewModelType != this.GetType()).ToList();
    }

    [ObservableProperty]
    private List<User> _allUser = new() {new User("User 1"),new User("User 2"), new User("User 3") , new User("User 4") };

    [ObservableProperty]
    public List<User> _yourTeam;

    [RelayCommand]
    public void CreateTeam() 
    {
        YourTeam = new();
       
        for (int i = 0; i < AllUser.Count; i++) 
        {
            if (AllUser[i].HasTeam) 
            {
                if (!YourTeam.Contains(AllUser[i]))
                {
                    YourTeam.Add(AllUser[i]);
                }
            }
        }

    }

    [RelayCommand]
    public void BackToPage() 
    {
        _navigationService.CurrentPageType = typeof(BalancingPage);

    }
}
