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
                return UserName;
            }
            set
            {
                UserName = value;
                Inital = Char.ToUpper(value[0], CultureInfo.CreateSpecificCulture("en-US"));
            }
        }

        /// <summary>
        /// The inital of a username
        /// </summary>
        public char Inital { get; set; }

        /// <summary>
        /// Hash of the user's password
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Salt of the user's encryption key
        /// </summary>
        public byte[] Salt { get; set; }
    }
}
