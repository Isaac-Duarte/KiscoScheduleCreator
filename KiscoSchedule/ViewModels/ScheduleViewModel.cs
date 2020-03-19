using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Services;
using KiscoSchedule.Shared.Models;
using Microsoft.Win32;
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
using System.Windows.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace KiscoSchedule.ViewModels
{
    class ScheduleViewModel : Screen
    {
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;
        private AsyncObservableCollection<IEmployee> employees;
        private static AsyncObservableCollection<IShift> shifts;
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
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));

            schedule = await _databaseService.GetScheduleAsync(date);

            if (schedule.Id == 0 || schedule.Date == null)
            {
                schedule.UserId = _user.Id;
                schedule.Date = SelectedDate;
                schedule.Shifts = new Dictionary<int, ShiftTemplate>();

                schedule.Id = (int)await _databaseService.CreateScheduleAsync(_user, schedule);
            }

            await Task.Run(() =>
            {
                _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));
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
                            }

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
            });


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

        public void ComboBoxChange(object sender, object dataContext, SelectionChangedEventArgs args, DayOfWeek day)
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
        }

        public async void Save()
        {
            await _databaseService.UpdateScheduleAsync(schedule);
        }

        public void Export()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel file (*.xls)|*.xls";

            if (saveFileDialog.ShowDialog() == true)
            {
                Excel._Application app = new Excel.Application();
                Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                Excel._Worksheet worksheet;

                worksheet = workbook.Sheets["Sheet1"];
                worksheet = workbook.ActiveSheet;

                worksheet.Name = Month;

                worksheet.Cells[1, 1] = SelectedDate.ToString("MMMM");

                for (int x = 1; x < 8; x++)
                {
                    worksheet.Cells[1, x + 1] = SelectedDate.AddDays((long)x - 1).ToString("dd");
                }

                worksheet.Cells[3, 1] = "Employee";

                int i = 1;

                foreach (DayOfWeek dayOfWeek in (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek)))
                {
                    i++;

                    worksheet.Cells[2, i] = dayOfWeek.ToString();
                }

                i = 2;
                foreach (Employee employee in Employees)
                {
                    i++;

                    worksheet.Cells[i, 1] = employee.Name;
                    worksheet.Cells[i, 2] = employee.Sunday.Name;
                    worksheet.Cells[i, 3] = employee.Monday.Name;
                    worksheet.Cells[i, 4] = employee.Tuesday.Name;
                    worksheet.Cells[i, 5] = employee.Wednesday.Name;
                    worksheet.Cells[i, 6] = employee.Thursday.Name;
                    worksheet.Cells[i, 7] = employee.Friday.Name;
                    worksheet.Cells[i, 8] = employee.Saturday.Name;
                }

                worksheet.Columns.AutoFit();

                Excel.Range last = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
                Excel.Range range = worksheet.get_Range("A1", last);
                range.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                workbook.SaveAs(saveFileDialog.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                app.Quit();
            }
        }
    }
}
