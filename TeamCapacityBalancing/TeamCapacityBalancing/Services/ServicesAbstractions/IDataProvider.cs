using System.Collections.Generic;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.Services.ServicesAbstractions
{
    public interface IDataProvider
    {
        public List<IssueData> GetAllEpics(string teamLeaderUsername);
        public List<IssueData> GetAllStories(string teamLeaderUsername);
        public List<User> GetAllUsers();
        public List<IssueData> GetStoriesByEpic(string epicId);
    }
}
