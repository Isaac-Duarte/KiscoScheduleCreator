﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Models
{
    public class Role : IRole
    {
        /// <summary>
        /// SQLite Id of the Employee
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the role
        /// </summary>
        public string Name{ get; set; }
    }
}
