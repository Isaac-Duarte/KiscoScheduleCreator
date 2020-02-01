using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Services;
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
        private AsyncObservableCollection<IEmployee> employees;

        public EmployeeViewModel(IDatabaseService databaseHelper, IEventAggregator events, IUser user)
        {
            _databaseService = databaseHelper;
            _events = events;
            _user = user;

            employees = new AsyncObservableCollection<IEmployee>();
            loadEmployeesAsync(employees.Count, 50);
        }

        /// <summary>
        /// Loads the employees
        /// </summary>
        public async void loadEmployeesAsync(int offset, int limit)
        {
            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Visible));
            
            List<IEmployee> newEmployees = await _databaseService.GetEmployeesAsync(_user, limit, offset);

            Employees.AddRange(newEmployees);

            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Collapsed));
        }

        public AsyncObservableCollection<IEmployee> Employees
        {
            get { return employees; }
            set
            {
                employees = value;
                NotifyOfPropertyChange(() => Employees);
            }
        }
    }
}
