using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public class Role : IRole
    {
        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the role
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of Shifts
        /// </summary>
        public List<Shift> Shifts { get; set; }

        /// <summary>
        /// Converts shifts to json
        /// </summary>
        /// <param name="shifts"></param>
        /// <returns></returns>
        public static string ConvertShifts(List<Shift> shifts)
        {
            return JsonConvert.SerializeObject(shifts);
        }

        /// <summary>
        /// Converts json to shifts
        /// </summary>
        /// <param name="rawShifts"></param>
        /// <returns></returns>
        public static List<Shift> ConvertShifts(string rawShifts)
        {
            return JsonConvert.DeserializeObject<List<Shift>>(rawShifts);
        }
    }
}
