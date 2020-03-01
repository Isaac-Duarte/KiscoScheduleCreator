using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public class Shift : IShift
    {
        /// <summary>
        /// SQLite Id of the shift
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the parent
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The name of the shift
        /// </summary>
        public string Name { get; set; }
    }
}
