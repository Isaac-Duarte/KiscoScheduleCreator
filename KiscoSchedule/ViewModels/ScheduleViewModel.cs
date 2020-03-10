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
using System.Windows.Controls;

namespace KiscoSchedule.ViewModels
{
    class ScheduleViewModel : Screen
    {
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;
        private AsyncObservableCollection<IEmployee> employees;
        private static AsyncObservableCollection<IShift> shifts;
        private bool busyAddingEmployees;

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
            loadEmployeesAsync();

            employees = new AsyncObservableCollection<IEmployee>();
            busyAddingEmployees = false;
        }

        /// <summary>
        /// Loads the schedule async
        /// </summary>
        private async void LoadSchedule()
        {
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));
            Shifts = new AsyncObservableCollection<IShift>(await _databaseService.GetShiftsAsync(_user));
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Collapsed));
        }

        /// <summary>
        /// Loads the employees
        /// </summary>
        public async void loadEmployeesAsync()
        {
            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Visible));

            List<IEmployee> newEmployees = await _databaseService.GetEmployeesAsync(_user);

            await Task.Run(() => newEmployees.ForEach(x =>
            {
                x.Monday = Shifts[0];
                x.Sunday = Shifts[0];
                x.Tuesday = Shifts[0];
                x.Wednesday = Shifts[0];
                x.Thursday = Shifts[0];
                x.Friday = Shifts[0];
                x.Saturday = Shifts[0];
            }));

            await Task.Run(() => Employees.AddRange(newEmployees));

            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Collapsed));

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
        static public AsyncObservableCollection<IShift> Shifts
        {
            get { return shifts; }
            set
            {
                shifts = value;
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

        public void ComboBoxChange(object sender, SelectionChangedEventArgs args)
        {

        }
    }
}
