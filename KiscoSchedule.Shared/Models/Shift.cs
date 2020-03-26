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
        /// The start of the shift
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// The end of the shift
        /// </summary>
        public DateTime End { get; set; }

        public string Name
        {
            get
            {
                if (Start == null || End == null)
                {
                    return "xx";
                }

                return $"{Start.ToString("t")} - {End.ToString("t")}";
            }
        }
    }
}
