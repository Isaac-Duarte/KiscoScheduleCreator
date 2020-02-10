using System;
using System.Collections.Generic;

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
        /// The inital of the name of the uesr
        /// </summary>
        char Inital { get; set; }

        /// <summary>
        /// The phone number of the employee
        /// </summary>
        string PhoneNumber { get; set; }
    }
}