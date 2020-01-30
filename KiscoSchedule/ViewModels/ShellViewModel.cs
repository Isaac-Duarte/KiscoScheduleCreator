using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiscoSchedule.Database.Services;
using KiscoSchedule.Shared.Util;

namespace KiscoSchedule.ViewModels
{
    class ShellViewModel : Conductor<object>
    {
        private IEventAggregator _events;
        private SimpleContainer _container;

        public ShellViewModel(IEventAggregator events, SimpleContainer container)
        {
            // Locally set the singletons
            _events = events;
            _container = container;

            DatabaseService database = new DatabaseService(FileUtil.GetAppDataFolder() + @"\KiscoSchedule", "database.db");
            
            database.OpenAsync();
        }
    }
}