using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KiscoSchedule.ViewModels
{
    class ScheduleViewModel : Screen
    {
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;
        private List<IEmployee> employees;

        /// <summary>
        /// Constuctor for ScheduleViewModel
        /// </summary>
        public ScheduleViewModel(IDatabaseService databaseService, IEventAggregator events, IUser user)
        {
            _databaseService = databaseService;
            _events = events;
            _user = user;

            // Load the schedule
            LoadSchedule();
        }

        private async void LoadSchedule()
        {
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));
            employees = await _databaseService.GetEmployeesAsync(_user);
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Collapsed));
        }

        public List<IEmployee> Employees
        {
            get { return employees; }
            set
            {
                employees = value;
                NotifyOfPropertyChange(() => Employees);
            }
        }

        public string Month
        {
            get
            {
                return DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);
            }
        }
    }
}
