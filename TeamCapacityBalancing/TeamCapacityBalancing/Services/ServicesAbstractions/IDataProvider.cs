using System.Collections.Generic;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.Services.ServicesAbstractions
{
    public interface IDataProvider
    {
        public List<IssueData> GetAllEpicsByTeamLeader(User teamLeader);
        public List<IssueData> GetAllStoriesByTeamLeader(User teamLeader);
        public List<User> GetAllUsers();
        public List<IssueData> GetStoriesByEpic(int epicId);
        public List<OpenTasksUserAssociation> GetRemainingForUser();
        public List<User> GetAllTeamLeaders();
    }
}
