using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Services;
using KiscoSchedule.Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KiscoSchedule.ViewModels
{
    public class TemplateViewModel : Screen
    {
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;
        private AsyncObservableCollection<IEmployee> employees;
        private static AsyncObservableCollection<IShift> shifts;
        private ISchedule schedule;
        private DateTime selectedDate;

        /// <summary>
        /// Constuctor for ScheduleViewModel
        /// </summary>
        public TemplateViewModel(IDatabaseService databaseService, IEventAggregator events, IUser user)
        {
            _databaseService = databaseService;
            _events = events;
            _user = user;

            employees = new AsyncObservableCollection<IEmployee>();
            SelectedDate = new DateTime(2003, 6, 13);

            loadSchedule();
        }

        /// <summary>
        /// Loads the schedule async
        /// </summary>
        private async void loadSchedule()
        {
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));
           
            // Load employees
            Employees = new AsyncObservableCollection<IEmployee>(await _databaseService.GetEmployeesAsync(_user));

            await loadTemplate();

            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Collapsed));
        }

        /// <summary>
        /// Loads a template
        /// </summary>
        private async Task loadTemplate()
        {
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));

            // My birthday ;0
            schedule = await _databaseService.GetScheduleAsync(SelectedDate);

            if (schedule.Id == 0 || schedule.Date == null)
            {
                schedule.UserId = _user.Id;
                schedule.Date = SelectedDate;
                schedule.Shifts = new Dictionary<int, ShiftTemplate>();

                schedule.Id = (int)await _databaseService.CreateScheduleAsync(_user, schedule);
            }

            CollectionViewSource.GetDefaultView(Employees).Refresh();

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
        static public AsyncObservableCollection<IShift> Shifts
        {
            get { return shifts; }
            set
            {
                shifts = value;
            }
        }

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { selectedDate = value; }
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

        public async void Save()
        {
            await _databaseService.UpdateScheduleAsync(schedule);
        }
    }
}
