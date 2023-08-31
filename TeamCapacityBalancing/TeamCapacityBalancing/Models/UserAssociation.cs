using CommunityToolkit.Mvvm.ComponentModel;

namespace TeamCapacityBalancing.Models;


public sealed partial class UserAssociation : ObservableObject
{
    public User User { get; set; }
    public float Days { get; set; }
    public bool IsShow { get; set; }

    public UserAssociation(User user, float days, bool isShow)
    {
        User = user;
        Days = days;
        IsShow = isShow;
    }
}
