using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.Services.ServicesAbstractions
{
    interface IDataSerialization
    {
        public void SerializeData(List<UserStoryDataSerialization> userStoryDataSerializations, string filename);
        List<UserStoryDataSerialization> DeserializeData(string filename);
    }
}
