using System;
using System.Collections.Generic;

namespace KiscoSchedule.Shared.Models
{
    public class Schedule : ISchedule
    {
        /// <summary>
        /// The employee id
        /// </summary>
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// The shifts of the employee
        /// </summary>
        public Dictionary<int, ShiftTemplate> Shifts { get; set; }
    }

    public class ShiftTemplate : IShiftTemplate
    {
        public Dictionary<DayOfWeek, Shift> Shifts { get; set; }
    }

    public interface IShiftTemplate
    {
        Dictionary<DayOfWeek, Shift> Shifts { get; set; }
    }
}