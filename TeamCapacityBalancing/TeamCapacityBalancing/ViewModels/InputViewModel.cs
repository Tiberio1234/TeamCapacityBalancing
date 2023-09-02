using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TeamCapacityBalancing.Navigation;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class InputViewModel : ObservableObject
{
    public ServiceCollection? _serviceCollection;
    public int hours = 0;

    public InputViewModel()
    {
    }

    public InputViewModel(ServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    [RelayCommand]
    public void AtClose()
    {
        if (_serviceCollection != null)
        {
            var vm = _serviceCollection.GetService(typeof(TeamViewModel));
            if (vm != null)
            {
                ((TeamViewModel)vm).SelectedUserYourTeam.HoursPerDay = hours;
            }
        }
    }
}
