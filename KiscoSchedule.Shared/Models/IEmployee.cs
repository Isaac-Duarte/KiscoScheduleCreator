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

        IShift Sunday { get; set; }
        IShift Monday { get; set; }
        IShift Tuesday { get; set; }
        IShift Wednesday { get; set; }
        IShift Thursday { get; set; }
        IShift Friday { get; set; }
        IShift Saturday { get; set; }
    }
}