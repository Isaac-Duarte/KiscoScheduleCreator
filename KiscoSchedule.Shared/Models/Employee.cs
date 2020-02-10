﻿using System;
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
        public long Id { get; set; }

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
    }
}
