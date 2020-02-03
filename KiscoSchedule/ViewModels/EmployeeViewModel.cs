using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Services;
using KiscoSchedule.Shared.Models;
using KiscoSchedule.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KiscoSchedule.ViewModels
{
    class EmployeeViewModel : Screen
    {
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;
        private AsyncObservableCollection<IEmployee> employees;
        private Employee selectedEmployee;

        public EmployeeViewModel(IDatabaseService databaseHelper, IEventAggregator events, IUser user)
        {
            _databaseService = databaseHelper;
            _events = events;
            _user = user;

            employees = new AsyncObservableCollection<IEmployee>();
            loadEmployeesAsync(employees.Count, 30);
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

        /// <summary>
        /// The collection that contains employees
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
        /// The selected employee in the listview
        /// </summary>
        public Employee SelectedEmployee
        {
            get { return selectedEmployee; }
            set
            {
                selectedEmployee = value;
                NotifyOfPropertyChange(() => SelectedEmployee);
            }
        }

        /// <summary>
        /// Determains weather somebody can add a 
        /// </summary>
        public bool CanAdd
        {
            get { return true; }
        }

        public List<string> ListDays
        {
            get
            {
                return Enum.GetNames(typeof(DayOfWeek)).ToList<string>();
            }
        }

        /// <summary>
        /// Event for the add button
        /// </summary>
        public async void Add()
        {
            Employee employee = new Employee
            {
                Name = "New Employee",
                PhoneNumber = "N/A",
                PerferedWorkingDays = new List<DayOfWeek>(),
                UnableWeekDays = new List<DayOfWeek>(),
                UnableSpecificDays = new List<DateTime>(),
                Roles = new List<Role>()
            };

            await _databaseService.CreateEmployeeAsync(_user, employee);

            Employees.Add(employee);

            SelectedEmployee = employee;
        }

        /// <summary>
        /// Removes the employee
        /// </summary>
        public async void RemoveClick(object dataContext)
        {
                Employees.Remove(SelectedEmployee);
        }

        public async void PreferredDialog(object dataContext)
        {
            Employee employee = dataContext as Employee;

            var view = new PreferredDialogView
            {
                DataContext = new PreferredDialogViewModel(employee.PerferedWorkingDays)
            };

            var result = await DialogHost.Show(view, "RootDialog");

            employee.PerferedWorkingDays = (List<DayOfWeek>) result;
        }
    }
}
