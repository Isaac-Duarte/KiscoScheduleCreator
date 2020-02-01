using System;
using System.Collections.Generic;

namespace KiscoSchedule.Shared.Models
{
    public interface IEmployee
    {
        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// SQLite Id of the User which owns this employee
        /// </summary>
        int UserId { get; set; }

        /// <summary>
        /// The name of the employee
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The inital of the name of the uesr
        /// </summary>
        char Inital { get; set; }

        /// <summary>
        /// The phone number of the employee
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// Perfered working days
        /// </summary>
        List<DayOfWeek> PerferedWorkingDays { get; set; }

        /// <summary>
        /// Unable week days
        /// </summary>
        List<DayOfWeek> UnableWeekDays { get; set; }

        /// <summary>
        /// Unable specific working days
        /// </summary>
        List<DateTime> UnableSpecificDays { get; set; }

        /// <summary>
        /// The roles of the user
        /// </summary>
        List<Role> Roles { get; set; }

        /// <summary>
        /// Converts a list of dayofweek to json
        /// </summary>
        /// <param name="weekDays">list wanting to be converted</param>
        /// <returns>converted string</returns>
        string ConvertWeekDays(List<DayOfWeek> weekDays);

        /// <summary>
        /// Converts json to day of week list
        /// </summary>
        /// <param name="weekDaysRaw"></param>
        /// <returns></returns>
        List<DayOfWeek> ConvertWeekDays(string weekDaysRaw);

        /// <summary>
        /// Converts a list of DateTime to json
        /// </summary>
        /// <param name="unableSpecificDays"></param>
        /// <returns></returns>
        string ConvertUnableSpecificDays(List<DateTime> unableSpecificDays);

        /// <summary>
        /// Converts JSON to a list of DateTime
        /// </summary>
        /// <param name="unableSpecificDaysRaw"></param>
        /// <returns></returns>
        List<DateTime> ConvertUnableSpecificDays(string unableSpecificDaysRaw);
    }
}