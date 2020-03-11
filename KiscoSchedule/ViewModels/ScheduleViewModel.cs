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
        private DateTime selectedDate;

        /// <summary>
        /// Constuctor for ScheduleViewModel
        /// </summary>
        public ScheduleViewModel(IDatabaseService databaseService, IEventAggregator events, IUser user)
        {
            _databaseService = databaseService;
            _events = events;
            _user = user;

            // Load the schedule
            loadSchedule();
            loadEmployeesAsync();

            employees = new AsyncObservableCollection<IEmployee>();
            busyAddingEmployees = false;
            SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Loads the schedule async
        /// </summary>
        private async void loadSchedule()
        {
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));
            Shifts = new AsyncObservableCollection<IShift>(await _databaseService.GetShiftsAsync(_user));
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Collapsed));
        }

        /// <summary>
        /// Loads a certain template
        /// </summary>
        /// <param name="date"></param>
        private async void loadTemplate(DateTime date)
        {

        }

        /// <summary>
        /// Loads the employees
        /// </summary>
        public async void loadEmployeesAsync()
        {
            _events.PublishOnUIThread(new ProgressEventModel(System.Windows.Visibility.Visible));

            List<IEmployee> newEmployees = await _databaseService.GetEmployeesAsync(_user);

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
                return SelectedDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + SelectedDate.ToString("dd-") + SelectedDate.AddDays(6).ToString("dd");
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value.AddDays(-(int)value.DayOfWeek);
                NotifyOfPropertyChange(() => SelectedDate);
                NotifyOfPropertyChange(() => Month);
            }
        }

        public void ComboBoxChange(object sender, SelectionChangedEventArgs args)
        {

        }
    }
}
