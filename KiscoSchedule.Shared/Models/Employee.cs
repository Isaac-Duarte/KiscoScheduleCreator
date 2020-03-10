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

        public IShift Sunday { get; set; }
        public IShift Monday { get; set; }
        public IShift Tuesday { get; set; }
        public IShift Wednesday { get; set; }
        public IShift Thursday { get; set; }
        public IShift Friday { get; set; }
        public IShift Saturday { get; set; }
    }
}
