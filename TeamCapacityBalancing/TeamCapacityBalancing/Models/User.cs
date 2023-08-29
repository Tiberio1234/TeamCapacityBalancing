using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

﻿namespace TeamCapacityBalancing.Models
{
    public class User
    {
        public string Username { get; set; }
      
        public string DisplayName { get; set; }
      
        public bool HasTeam { get; set; }
      
        public int Id { get; set; }
      
        public User(string username, string displayName, int id)
        {
            Username = username;
            DisplayName = displayName;
            Id = id;
        }
    }
}
