using System;
using System.Collections.Generic;

namespace KiscoSchedule.Shared.Models
{
    public class Template : ITemplate
    {
        /// <summary>
        /// The employee id
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// The shifts of the employee
        /// </summary>
        public Dictionary<DayOfWeek, int> Shifts { get; set; }
    }
}