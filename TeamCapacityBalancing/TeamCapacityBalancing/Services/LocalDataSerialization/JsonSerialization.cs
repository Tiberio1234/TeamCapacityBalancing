using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Services.ServicesAbstractions;

//Test:
/*User user1 = new User("andu", "tibi", 1);
                User user2 = new User("druga", "andrei", 2);
                Tuple<User, float> tuple1 = new Tuple<User, float>(user1, 3);
                Tuple<User, float> tuple2 = new Tuple<User, float>(user2, 4);
                List<User> users = new List<User>();
                users.Add(user1);
                users.Add(user2);


                UserStoryDataSerialization userStoryDataSerialization1 = new UserStoryDataSerialization();
                UserStoryDataSerialization userStoryDataSerialization2 = new UserStoryDataSerialization();
                userStoryDataSerialization1.StoryId = 0;
                userStoryDataSerialization2.StoryId = 1;
                userStoryDataSerialization1.ShortTerm = true;
                userStoryDataSerialization1.ShortTerm = false;

                userStoryDataSerialization1.UsersCapacity.Add(tuple1);
                userStoryDataSerialization1.UsersCapacity.Add(tuple2);
                userStoryDataSerialization2.UsersCapacity.Add(tuple2);

                JsonSerialization jsonSerialization = new JsonSerialization();
                List<UserStoryDataSerialization> userStoryDataSerializations = new List<UserStoryDataSerialization>();
                userStoryDataSerializations.Add(userStoryDataSerialization1);
                userStoryDataSerializations.Add(userStoryDataSerialization2);

                jsonSerialization.SerializeUserStoryData(userStoryDataSerializations, "tibi");
                List<UserStoryDataSerialization> test = jsonSerialization.DeserializeUserStoryData("tibi");

                jsonSerialization.SerializeTeamData(users,"tibi'steam");
                jsonSerialization.DeserializeTeamData("tibi'steam");*/



namespace TeamCapacityBalancing.Services.LocalDataSerialization
{
    public class JsonSerialization : IDataSerialization
    {
        private const string UserStoryFilePath = "../../../LocalFiles/UserStoryData/";
        private const string UserFilePath = "../../../LocalFiles/TeamData/";

        public List<User> DeserializeTeamData(string filename)
        {
            return JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(UserFilePath + filename));
        }

        public List<UserStoryDataSerialization> DeserializeUserStoryData(string filename)
        {
            return JsonConvert.DeserializeObject<List<UserStoryDataSerialization>>(File.ReadAllText(UserStoryFilePath + filename));
        }

        public void SerializeTeamData(List<User> userDataSerializations, string filename)
        {
            File.WriteAllText(UserFilePath + filename, JsonConvert.SerializeObject(userDataSerializations));
        }

        public void SerializeUserStoryData(List<UserStoryDataSerialization> userStoryDataSerializations,string filename)
        {
            File.WriteAllText(UserStoryFilePath + filename, JsonConvert.SerializeObject(userStoryDataSerializations));
        }
    }
}
