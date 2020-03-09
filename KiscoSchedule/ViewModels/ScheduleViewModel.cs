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
        private AsyncObservableCollection<IShift> shifts;
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
        public async Task<int> loadEmployeesAsync(int offset, int limit)
        {
            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Visible));

            List<IEmployee> newEmployees = await _databaseService.GetEmployeesAsync(_user, limit, offset);

            newEmployees.ForEach(employee =>
            {
                employee.Shifts = Shifts;
            });

            Employees.AddRange(newEmployees);

            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Collapsed));

            return newEmployees.Count;
        }

        /// <summary>
        /// Called when the datagrid is scolled
        /// </summary>
        /// <param name="e"></param>
        public async void DoScroll(ScrollChangedEventArgs e)
        {
            var scrollViewer = e.OriginalSource as ScrollViewer;
            if (scrollViewer != null && scrollViewer.ScrollableHeight >= 0 && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight && !busyAddingEmployees)
            {
                busyAddingEmployees = true;
                int amount = await loadEmployeesAsync(Employees.Count, 10);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 1d);

                if (amount > 0)
                {
                    busyAddingEmployees = false;
                }
            }
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
