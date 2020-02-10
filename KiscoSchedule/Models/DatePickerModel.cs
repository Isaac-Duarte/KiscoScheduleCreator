using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Models
{
    class DatePickerModel
    {
        private DateTime dateTime;

        public DatePickerModel(DateTime dateTime)
        {
            DateTime = dateTime;
        }
        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }
    }
}
