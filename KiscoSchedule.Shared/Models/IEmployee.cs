using System;
using System.Collections.Generic;

namespace KiscoSchedule.Shared.Models
{
    public interface IEmployee
    {
        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// SQLite Id of the User which owns this employee
        /// </summary>
        int UserId { get; set; }

        /// <summary>
        /// The name of the employee
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The phone number of the employee
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// Perfered working days
        /// </summary>
        List<DayOfWeek> PerferedWorkingDays { get; set; }

        /// <summary>
        /// Unable week days
        /// </summary>
        List<DayOfWeek> UnableWeekDays { get; set; }

        /// <summary>
        /// Unable specific working days
        /// </summary>
        List<DateTime> UnableSpecificDays { get; set; }

        /// <summary>
        /// The roles of the user
        /// </summary>
        List<Role> Roles { get; set; }
    }
}