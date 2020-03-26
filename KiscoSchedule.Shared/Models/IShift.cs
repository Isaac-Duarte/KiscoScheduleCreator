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
        /// The start of the shift
        /// </summary>
        DateTime Start { get; set; }

        /// <summary>
        /// The end of the shift
        /// </summary>
        DateTime End { get; set; }

        string Name { get; }
    }
}
