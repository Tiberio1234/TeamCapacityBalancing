using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Services.ServicesAbstractions;

namespace TeamCapacityBalancing.Services.LocalDataSerialization
{
    internal class JsonSerialization : IDataSerialization
    {
        public List<UserStoryDataSerialization> DeserializeData(string filename)
        {

            return JsonConvert.DeserializeObject<List<UserStoryDataSerialization>>(File.ReadAllText("../../" + filename));
         
        }

        public void SerializeData(List<UserStoryDataSerialization> userStoryDataSerializations,string filename)
        {
            File.WriteAllText("../../" + filename, JsonConvert.SerializeObject(userStoryDataSerializations));
        }
    }
}
