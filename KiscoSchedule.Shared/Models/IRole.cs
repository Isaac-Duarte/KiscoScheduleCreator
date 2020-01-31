using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    interface IRole
    {
        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Name of the role
        /// </summary>
        string Name { get; set; }
    }
}
