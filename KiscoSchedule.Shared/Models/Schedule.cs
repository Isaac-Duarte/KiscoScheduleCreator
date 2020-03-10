using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public class Schedule
    {
        /// <summary>
        /// This is the SQL Id of the schedule
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This is the starting date of the schedule
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Templates of an employee
        /// </summary>
        public ObservableCollection<Template> EmployeeTemplates { get; set; }
    }
}
