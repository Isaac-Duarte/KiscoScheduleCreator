using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KiscoSchedule.Shared.Models
{
    public interface IEmployee
    {
        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        long Id { get; set; }

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
        /// The shfits of the employee
        /// </summary>
        ObservableCollection<IShift> Shifts { get; set; }

        Dictionary<DayOfWeek, IShift> ShiftsWeek { get; set; }
    }
}