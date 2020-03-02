using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public interface IShift
    {
        /// <summary>
        /// SQLite Id of the shift
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The Id of the parent
        /// </summary>
        int UserId { get; set; }

        /// <summary>
        /// The name of the shift
        /// </summary>
        string Name { get; set; }
    }
}
