using Caliburn.Micro;
using KiscoSchedule.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.ViewModels
{
    public class PreferredDialogViewModel : Screen
    {
        private List<DayOfWeekCheck> days;

        public PreferredDialogViewModel(List<DayOfWeek> preferredWorkingDays)
        {
            days = new List<DayOfWeekCheck>();

            foreach (string day in Enum.GetNames(typeof(DayOfWeek)).ToList<string>())
            {
                DayOfWeek dayOfWeek = (DayOfWeek) Enum.Parse(typeof(DayOfWeek), day);
                DayOfWeekCheck dayOfWeekCheck = new DayOfWeekCheck
                {
                    Day = dayOfWeek.ToString()
                };

                if (preferredWorkingDays.Contains(dayOfWeek))
                {
                    dayOfWeekCheck.IsChecked = true;
                }

                Days.Add(dayOfWeekCheck);
            }
        }

        /// <summary>
        /// Collection for the days of the week
        /// </summary>
        public List<DayOfWeekCheck> Days
        {
            get
            {
                return days;
            }
            set
            {
                days = value;
                NotifyOfPropertyChange(() => Days);
            }
        }

        public void Submit()
        {
            DialogHost.CloseDialogCommand.Execute(this, null);
        }
    }
}
