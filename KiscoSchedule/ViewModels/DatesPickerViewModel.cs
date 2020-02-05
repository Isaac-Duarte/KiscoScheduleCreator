using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.ViewModels
{
    class DatesPickerViewModel : Screen
    {
        private List<DateTime> dateTimes;

        public DatesPickerViewModel(List<DateTime> dateTimes)
        {
            DateTimes = dateTimes;
        }

        public List<DateTime> DateTimes
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
