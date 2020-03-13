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
using System.Reflection;
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
        private ISchedule schedule;

        /// <summary>
        /// Constuctor for ScheduleViewModel
        /// </summary>
        public ScheduleViewModel(IDatabaseService databaseService, IEventAggregator events, IUser user)
        {
            _databaseService = databaseService;
            _events = events;
            _user = user;

            employees = new AsyncObservableCollection<IEmployee>();
            busyAddingEmployees = false;
            SelectedDate = DateTime.Now;

            // Load the schedule
            loadSchedule();
            loadSchedule(SelectedDate);
        }

        /// <summary>
        /// Loads the schedule async
        /// </summary>
        private async void loadSchedule()
        {
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));
            // Load shifts
            Shifts = new AsyncObservableCollection<IShift>(await _databaseService.GetShiftsAsync(_user));

            // Load employees
            List<IEmployee> newEmployees = await _databaseService.GetEmployeesAsync(_user);

            await Task.Run(() => Employees.AddRange(newEmployees));

            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Collapsed));
        }

        /// <summary>
        /// Loads a certain template
        /// </summary>
        /// <param name="date"></param>
        private async void loadSchedule(DateTime date)
        {
            schedule = await _databaseService.GetScheduleAsync(date);

            if (schedule.Id == 0 || schedule.Date == null)
            {
                schedule.UserId = _user.Id;
                schedule.Date = SelectedDate;
                schedule.Shifts = new Dictionary<int, ShiftTemplate>();

                await _databaseService.CreateScheduleAsync(_user, schedule);
            }

            await Task.Run(() =>
            {
                foreach (IEmployee employee in Employees)
                {
                    if (schedule.Shifts.ContainsKey((int)employee.Id))
                    {
                        try
                        {
                            employee.Sunday = Shifts.Where(x => x.Id == schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Sunday]).FirstOrDefault();
                            employee.Monday = Shifts.Where(x => x.Id == schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Monday]).FirstOrDefault();
                            employee.Tuesday = Shifts.Where(x => x.Id == schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Tuesday]).FirstOrDefault();
                            employee.Wednesday = Shifts.Where(x => x.Id == schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Wednesday]).FirstOrDefault();
                            employee.Thursday = Shifts.Where(x => x.Id == schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Thursday]).FirstOrDefault();
                            employee.Friday = Shifts.Where(x => x.Id == schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Friday]).FirstOrDefault();
                            employee.Saturday = Shifts.Where(x => x.Id == schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Saturday]).FirstOrDefault();
                        }
                        catch
                        {
                            
                        }
                    }
                    else
                    {
                        schedule.Shifts[(int)employee.Id] = new ShiftTemplate
                        {
                            Shifts = new Dictionary<DayOfWeek, int>()
                        };

                        if (Shifts.Count > 0)
                        {
                            foreach (DayOfWeek dayOfWeek in (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek)))
                            {
                                schedule.Shifts[(int)employee.Id].Shifts[dayOfWeek] = (int)Shifts[0].Id;
                                // Jesus I hate WPF (or maybe i'm stupid)
                                employee.Sunday = Shifts[0];
                                employee.Monday = Shifts[0];
                                employee.Tuesday = Shifts[0];
                                employee.Wednesday = Shifts[0];
                                employee.Thursday = Shifts[0];
                                employee.Friday = Shifts[0];
                                employee.Saturday = Shifts[0];
                            }
                        }
                    }
                }
            });
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

        /// <summary>
        /// The selected date for the schedule
        /// </summary>
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
                loadSchedule(selectedDate);
            }
        }

        public async void ComboBoxChange(object sender, object dataContext, SelectionChangedEventArgs args, DayOfWeek day)
        {
            IEmployee employee = (Employee)dataContext;
            IShift shift = (Shift)sender;

            if (!schedule.Shifts.ContainsKey((int)employee.Id))
            {
                schedule.Shifts[(int)employee.Id] = new ShiftTemplate
                {
                    Shifts = new Dictionary<DayOfWeek, int>()
                };

                if (Shifts.Count > 0)
                {
                    foreach (DayOfWeek dayOfWeek in (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek)))
                    {
                        schedule.Shifts[(int)employee.Id].Shifts[dayOfWeek] = (int)Shifts[0].Id;
                    }
                }
            }

            schedule.Shifts[(int)employee.Id].Shifts[day] = (int)shift.Id;

            await _databaseService.UpdateScheduleAsync(schedule);
        }
    }
}
