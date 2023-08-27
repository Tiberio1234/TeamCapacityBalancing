using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.ViewModels;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                QueriesForDataBase.GetAllUsers();

                /*User user1 = new User("andu","tibi", 1);
                User user2 = new User("druga","andrei",2);
                Tuple<User, float> tuple1 = new Tuple<User, float>(user1, 3);
                Tuple<User, float> tuple2 = new Tuple<User, float>(user2, 4);

                
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

                jsonSerialization.SerializeData(userStoryDataSerializations,"tibi");
                List<UserStoryDataSerialization> test = jsonSerialization.DeserializeData("tibi");*/

            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}