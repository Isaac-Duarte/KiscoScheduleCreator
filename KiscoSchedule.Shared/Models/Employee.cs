using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public class Employee : IEmployee
    {
        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// SQLite Id of the User which owns this employee
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The name of the employee
        /// </summary>
        public string Name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
                Inital = Char.ToUpper(value[0], CultureInfo.CreateSpecificCulture("en-US"));
            }
        }

        /// <summary>
        /// The inital of the name of the uesr
        /// </summary>
        public char Inital;

        /// <summary>
        /// The phone number of the employee
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Perfered working days
        /// </summary>
        public List<DayOfWeek> PerferedWorkingDays { get; set; }

        /// <summary>
        /// Unable week days
        /// </summary>
        public List<DayOfWeek> UnableWeekDays { get; set; }

        /// <summary>
        /// Unable specific working days
        /// </summary>
        public List<DateTime> UnableSpecificDays { get; set; }

        /// <summary>
        /// The roles of the user
        /// </summary>
        public List<Role> Roles { get; set; }
    }
}
