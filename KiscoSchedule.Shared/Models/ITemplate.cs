using System;
using System.Collections.Generic;

namespace KiscoSchedule.Shared.Models
{
    public interface ITemplate
    {
        /// <summary>
        /// The employee id
        /// </summary>
        int EmployeeId { get; set; }

        /// <summary>
        /// The shifts of the employee
        /// </summary>
        Dictionary<DayOfWeek, int> Shifts { get; set; }
    }
}