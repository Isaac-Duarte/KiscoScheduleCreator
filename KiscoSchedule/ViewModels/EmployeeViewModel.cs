using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.ViewModels
{
    class EmployeeViewModel : Screen
    {
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;
        private List<IEmployee> employees;

        public EmployeeViewModel(IDatabaseService databaseHelper, IEventAggregator events, IUser user)
        {
            _databaseService = databaseHelper;
            _events = events;
            _user = user;

            employees = new List<IEmployee>();
            loadEmployeesAsync(employees.Count, 10);
        }

        /// <summary>
        /// Loads the employees
        /// </summary>
        public async void loadEmployeesAsync(int offset, int limit)
        {
            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Visible));
            employees.AddRange(await _databaseService.GetEmployeesAsync(_user, limit, offset));
            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Collapsed));
        }
    }
}
