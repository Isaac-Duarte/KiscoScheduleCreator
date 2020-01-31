using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    interface IShift
    {
        /// <summary>
        /// SQLite Id of the shift.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Name of the shift.
        /// </summary>
        string Name { get; set; }
    }
}
