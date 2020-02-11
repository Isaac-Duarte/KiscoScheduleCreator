using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public class User : IUser
    {
        private string userName;

        /// <summary>
        /// This SQLite Id of the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username of the user
        /// </summary>
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }

        /// <summary>
        /// Hash of the user's password
        /// </summary>
        public string Hash { get; set; }
    }
}
