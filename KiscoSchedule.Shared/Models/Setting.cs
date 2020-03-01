using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public class Setting : ISetting
    {
        /// <summary>
        /// SQLite Id of the setting
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The SQLite UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The key string of the setting
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value of the setting
        /// </summary>
        public string Value { get; set; }
    }
}
