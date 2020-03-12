using System;
using System.Collections.Generic;

namespace KiscoSchedule.Shared.Models
{
    public interface ISchedule
    {
        /// <summary>
        /// The employee id
        /// </summary>
        int Id { get; set; }
        
        /// <summary>
        /// Id of the user
        /// </summary>
        int UserId { get; set; }

        DateTime Date { get; set; }

        /// <summary>
        /// The shifts of the employee
        /// </summary>
        Dictionary<int, ShiftTemplate> Shifts { get; set; }
    }
}