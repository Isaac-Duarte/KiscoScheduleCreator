using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public interface IRole
    {
        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Name of the role
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// List of Shifts
        /// </summary>
        List<Shift> Shifts { get; set; }

        /// <summary>
        /// Converts shifts to json
        /// </summary>
        /// <param name="shifts"></param>
        /// <returns></returns>
        string ConvertShifts(List<Shift> shifts);

        /// <summary>
        /// Converts json to shifts
        /// </summary>
        /// <param name="rawShifts"></param>
        /// <returns></returns>
        List<Shift> ConvertShifts(string rawShifts);
    }
}
