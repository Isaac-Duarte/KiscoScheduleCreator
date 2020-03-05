using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Services;
using KiscoSchedule.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private AsyncObservableCollection<IEmployee> employees;
        private AsyncObservableCollection<IShift> shifts;

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

        /// <summary>
        /// Loads the schedule async
        /// </summary>
        private async void LoadSchedule()
        {
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));
            Employees = new AsyncObservableCollection<IEmployee>(await _databaseService.GetEmployeesAsync(_user));
            Shifts = new AsyncObservableCollection<IShift>(await _databaseService.GetShiftsAsync(_user));
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Collapsed));
        }

        /// <summary>
        /// List of Employees
        /// </summary>
        public AsyncObservableCollection<IEmployee> Employees
        {
            get { return employees; }
            set
            {
                employees = value;
                NotifyOfPropertyChange(() => Employees);
            }
        }

        /// <summary>
        /// A collection of the user's sihfts
        /// </summary>
        public AsyncObservableCollection<IShift> Shifts
        {
            get { return shifts; }
            set
            {
                shifts = value;
                NotifyOfPropertyChange(() => Shifts);
            }
        }

        /// <summary>
        /// The current month
        /// </summary>
        public string Month
        {
            get
            {
                return DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);
            }
        }
    }
}
