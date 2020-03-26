using Caliburn.Micro;
using KiscoSchedule.Shared.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.ViewModels
{
    class ShiftDialogModel : Screen
    {
        private ISchedule schedule;
        private IEmployee employee;
        private DayOfWeek day;
        private DateTime start;
        private DateTime end;

        public ShiftDialogModel(ref ISchedule schedule, IEmployee employee, DayOfWeek day)
        {
            this.schedule = schedule;
            this.employee = employee;
            this.day = day;

            if (schedule.Shifts[(int)employee.Id].Shifts.ContainsKey(day))
            {
                Start = schedule.Shifts[(int)employee.Id].Shifts[day].Start;
                End = schedule.Shifts[(int)employee.Id].Shifts[day].End;
            }
        }

        public string Header
        {
            get
            {
                return $"{employee.Name} - {day.ToString()}";
            }
        }

        public DateTime Start
        {
            get { return start; }
            set
            {
                start = value;
                NotifyOfPropertyChange(() => Start);
            }
        }

        public DateTime End
        {
            get { return end; }
            set
            {
                end = value;
                NotifyOfPropertyChange(() => End);
            }
        }

        public void Save()
        {
            if (schedule.Shifts[(int)employee.Id].Shifts.ContainsKey(day))
            {
                schedule.Shifts[(int)employee.Id].Shifts[day].Start = Start;
                schedule.Shifts[(int)employee.Id].Shifts[day].End = End;
            }

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public void Cancel()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
