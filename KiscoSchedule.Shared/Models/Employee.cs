using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KiscoSchedule.Shared.Models
{
    public class Employee : IEmployee
    {
        private string name { get; set; }

        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// SQLite Id of the User which owns this employee
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The name of the employee
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                Inital = Char.ToUpper(value[0], CultureInfo.CreateSpecificCulture("en-US"));
            }
        }

        /// <summary>
        /// The inital of the name of the uesr
        /// </summary>
        public char Inital { get; set; }

        /// <summary>
        /// The phone number of the employee
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Perfered working days
        /// </summary>
        public List<DayOfWeek> PerferedWorkingDays { get; set; }

        /// <summary>
        /// Unable week days
        /// </summary>
        public List<DayOfWeek> UnableWeekDays { get; set; }

        /// <summary>
        /// Unable specific working days
        /// </summary>
        public List<DateTime> UnableSpecificDays { get; set; }

        /// <summary>
        /// The roles of the user
        /// </summary>
        public List<Role> Roles { get; set; }

        /// <summary>
        /// Converts a list of dayofweek to json
        /// </summary>
        /// <param name="weekDays">list wanting to be converted</param>
        /// <returns>converted string</returns>
        public static string ConvertWeekDays(List<DayOfWeek> weekDays)
        {
            return JsonConvert.SerializeObject(weekDays);
        }

        /// <summary>
        /// Converts json to day of week list
        /// </summary>
        /// <param name="weekDaysRaw"></param>
        /// <returns></returns>
        public static List<DayOfWeek> ConvertWeekDays(string weekDaysRaw)
        {
            return JsonConvert.DeserializeObject<List<DayOfWeek>>(weekDaysRaw);
        }

        /// <summary>
        /// Converts a list of DateTime to json
        /// </summary>
        /// <param name="unableSpecificDays"></param>
        /// <returns></returns>
        public static string ConvertUnableSpecificDays(List<DateTime> unableSpecificDays)
        {
            return String.Join(",", unableSpecificDays.ToArray());
        }

        /// <summary>
        /// Converts JSON to a list of DateTime
        /// </summary>
        /// <param name="unableSpecificDaysRaw"></param>
        /// <returns></returns>
        public static List<DateTime> ConvertUnableSpecificDays(string unableSpecificDaysRaw)
        {
            List<DateTime> unableSpecificDays = new List<DateTime>();

            foreach (string dateTime in unableSpecificDaysRaw.Split(','))
            {
                unableSpecificDays.Add(DateTime.Parse(dateTime));
            }

            return unableSpecificDays;
        }
    }
}
