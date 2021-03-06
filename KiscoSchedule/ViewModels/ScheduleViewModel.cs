﻿using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Services;
using KiscoSchedule.Shared.Enums;
using KiscoSchedule.Shared.Models;
using KiscoSchedule.Shared.Util;
using KiscoSchedule.Views;
using MaterialDesignThemes.Wpf;
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
        }

        /// <summary>
        /// Loads the schedule async
        /// </summary>
        private async void loadSchedule()
        {
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Visible));

            // Load employees
            List<IEmployee> newEmployees = await _databaseService.GetEmployeesAsync(_user);

            await Task.Run(() => Employees.AddRange(newEmployees));
            await loadSchedule(SelectedDate);
            _events.PublishOnUIThread(new ProgressEventModel(Visibility.Collapsed));
        }

        /// <summary>
        /// Loads a certain template
        /// </summary>
        /// <param name="date"></param>
        private async Task loadSchedule(DateTime date)
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
                foreach (IEmployee employee in Employees)
                {
                    if (schedule.Shifts.ContainsKey((int)employee.Id))
                    {
                        try
                        {
                            employee.Sunday = schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Sunday];
                            employee.Monday = schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Monday];
                            employee.Tuesday = schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Tuesday];
                            employee.Wednesday = schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Wednesday];
                            employee.Thursday = schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Thursday];
                            employee.Friday = schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Friday];
                            employee.Saturday = schedule.Shifts[(int)employee.Id].Shifts[DayOfWeek.Saturday];
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        schedule.Shifts[(int)employee.Id] = new ShiftTemplate
                        {
                            Shifts = new Dictionary<DayOfWeek, Shift>()
                        };
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

        public async void ChangeShift(object sender, object dataContext, DayOfWeek day)
        {
            IEmployee employee = (IEmployee)dataContext;
            
            var view = new ShiftDialog();

            view.DataContext = new ShiftDialogModel(ref schedule, employee, day);

            await DialogHost.Show(view);
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

        public async void TextMessage()
        {
            Dictionary<SettingEnum, ISetting> settings = await _databaseService.GetSettingsAsync(_user);

            await validateSetting(settings, SettingEnum.ACCOUNT_SID);
            await validateSetting(settings, SettingEnum.AUTH_TOKEN);
            await validateSetting(settings, SettingEnum.PHONE_NUMBER);
            await validateSetting(settings, SettingEnum.TEXT_MESSAGE);

            string twilioAccountSID = settings[SettingEnum.ACCOUNT_SID].Value;
            string twilioAuthToken = settings[SettingEnum.AUTH_TOKEN].Value;
            string twilioPhoneNumber = settings[SettingEnum.PHONE_NUMBER].Value;
            string textMessageReply = settings[SettingEnum.TEXT_MESSAGE].Value;

            SmsService smsService = new SmsService(twilioAccountSID, twilioAuthToken, twilioPhoneNumber);

            foreach (Employee employee in Employees)
            {
                string message = textMessageReply;
                
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append($"Sunday: {employee.Sunday.Name}\n");
                stringBuilder.Append($"Monday: {employee.Monday.Name}\n");
                stringBuilder.Append($"Tuesday: {employee.Tuesday.Name}\n");
                stringBuilder.Append($"Wednesday: {employee.Wednesday.Name}\n");
                stringBuilder.Append($"Thursday: {employee.Thursday.Name}\n");
                stringBuilder.Append($"Friday: {employee.Friday.Name}\n");
                stringBuilder.Append($"Saturday: {employee.Saturday.Name}\n");

                message = message.Replace("{employee}", employee.Name);
                message = message.Replace("{date}", Month);
                message = message.Replace("{schedule}", stringBuilder.ToString());

                smsService.SendMessage(employee.PhoneNumber, message);
            }
            _events.PublishOnUIThread(new SnackBarEventModel("Sent the text messages."));
        }

        private async Task validateSetting(Dictionary<SettingEnum, ISetting> settings, SettingEnum key)
        {
            if (!settings.ContainsKey(key))
            {
                Setting setting = new Setting
                {
                    Key = key,
                    Value = ""
                };

                long id = await _databaseService.CreateSettingAsync(_user, setting);
                setting.Id = (int)id;
                settings[key] = setting;
            }
        }

        public async void SetTemplate()
        {
            await loadSchedule(new DateTime(2003, 6, 13));
        }
    }
}
