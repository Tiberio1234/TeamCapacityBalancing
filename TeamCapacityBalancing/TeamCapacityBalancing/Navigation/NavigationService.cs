using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
namespace TeamCapacityBalancing.Navigation;

public sealed partial class NavigationService : ObservableObject
{
    [ObservableProperty]
    private bool _isNavigationAllowed = true;

    private Type? _currentPageType;

    public event Action<Type>? CurrentPageChanged;

    public List<User> team;

    public Type? CurrentPageType
    {
        get => _currentPageType;

        set
        {
            if (value is null)
            {
                return;
            }

            _currentPageType = value;

            CurrentPageChanged?.Invoke(value);
        }
    }
}
