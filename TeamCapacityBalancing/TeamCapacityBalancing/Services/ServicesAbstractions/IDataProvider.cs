using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.Services.ServicesAbstractions
{
    public interface IDataProvider
    {
        public Task<List<IssueData>> GetAllEpicAsync(string teamLeaderUsername);
        public Task<List<IssueData>> GetAllStoriesAsync(string teamLeaderUsername);
        public Task<List<IssueData>> GetAllUsersAsync();

        public Task<List<IssueData>> GetStoriesByEpicAsync(string epicId);


    }
}
