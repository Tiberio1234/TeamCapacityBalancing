using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models
{
    public class OpenTasksUserAssociation
    {

        public int UserId { get; set; }
        public string UserName { get; set; }
        public float Remaining { get; set; }

        public OpenTasksUserAssociation(int id, string username, float remaining)
        {
            UserId= id;
            UserName = username;
            Remaining= remaining;
        }
    }
}
