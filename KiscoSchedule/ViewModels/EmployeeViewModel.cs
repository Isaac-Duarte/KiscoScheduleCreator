﻿using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Models;
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
        private bool dataGridCurrentlyUpdating;

        public EmployeeViewModel(IDatabaseService databaseHelper, IEventAggregator events, IUser user)
        {
            _databaseService = databaseHelper;
            _events = events;
            _user = user;

            employees = new AsyncObservableCollection<IEmployee>();
            loadEmployeesAsync(employees.Count, 30);
            dataGridCurrentlyUpdating = false;
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

            long id = await _databaseService.CreateEmployeeAsync(_user, employee);
            employee.Id = id;

            Employees.Add(employee);

            SelectedEmployee = employee;
        }

        /// <summary>
        /// Removes the employee
        /// </summary>
        public async void RemoveClick(object dataContext)
        {
            if (SelectedEmployee == null)
                return;
            
            await _databaseService.DeleteEmployeeAsync(SelectedEmployee);
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
            PreferredDialogViewModel context = (PreferredDialogViewModel)result;

            List<DayOfWeek> perferedWorkingDays = new List<DayOfWeek>();

            foreach (DayOfWeekCheck dayOfWeekCheck in context.Days)
            {
                if (dayOfWeekCheck.IsChecked)
                {
                    perferedWorkingDays.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayOfWeekCheck.Day));
                }
            }

            employee.PerferedWorkingDays = perferedWorkingDays;
            await _databaseService.UpdateEmployeePerferedWorkingDaysAsync(employee, perferedWorkingDays);
        }

        /// <summary>
        /// Event for editing the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="employeeObj"></param>
        /// <param name="e"></param>
        public async void DataGrid_RowEditEnding(object sender, object employeeObj,  DataGridRowEditEndingEventArgs e)
        {
            if (sender == null)
                return;

            if (!dataGridCurrentlyUpdating)
            {
                dataGridCurrentlyUpdating = true;
                DataGrid dataGrid = (DataGrid)sender;

                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                dataGrid.Items.Refresh();
                dataGridCurrentlyUpdating = false;
            }

            if (e.EditAction == DataGridEditAction.Commit)
            {
                Employee employee = (Employee)employeeObj;

                await _databaseService.UpdateEmployeeAsync(employee);
            }
        }
    }
}
