using Caliburn.Micro;
using KiscoSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.ViewModels
{
    class DatesPickerViewModel : Screen
    {
        private List<DatePickerModel> dateTimes;

        public DatesPickerViewModel(List<DatePickerModel> dateTimes)
        {
            DateTimes = dateTimes;
        }

        /// <summary>
        /// List of Date Time for UnableSpecific Work Days
        /// </summary>
        public List<DatePickerModel> DateTimes
        {
            get { return dateTimes; }
            set
            {
                dateTimes = value;
                NotifyOfPropertyChange(() => DateTimes);
            }
        }
    }
}
