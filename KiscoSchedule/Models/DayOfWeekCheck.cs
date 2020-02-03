using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Models
{
    public class DayOfWeekCheck
    {
        private string day;
        private bool isChecked;

        public string Day
        {
            get
            {
                return day;
            }
            set
            {
                day = value;
            }
        }

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
            }
        }
    }
}
