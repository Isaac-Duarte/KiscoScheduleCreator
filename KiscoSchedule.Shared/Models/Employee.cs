using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KiscoSchedule.Shared.Models
{
    public class Employee : IEmployee
    {
        private ObservableCollection<IShift> shifts { get; set; }

        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// SQLite Id of the User which owns this employee
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The name of the employee
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The phone number of the employee
        /// </summary>
        public string PhoneNumber { get; set; }

        public ObservableCollection<IShift> Shifts 
        {
            get { return shifts; }
            set { shifts = value; }
        }

        public Dictionary<DayOfWeek, IShift> ShiftsWeek { get; set; }
    }
}
