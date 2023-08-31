using System.Collections.Generic;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.Navigation;
public class TeamService
{
    public List<User> Team { get; set; }

    public TeamService(List<User> users)
    {
        Team = users;
    }
}
