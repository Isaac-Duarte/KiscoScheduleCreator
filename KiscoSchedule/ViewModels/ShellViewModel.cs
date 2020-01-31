﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiscoSchedule.Database.Services;
using KiscoSchedule.Shared.Util;
using KiscoSchedule.Shared.Models;

namespace KiscoSchedule.ViewModels
{
    class ShellViewModel : Conductor<object>
    {
        private IEventAggregator _events;
        private SimpleContainer _container;

        public ShellViewModel(IEventAggregator events, SimpleContainer container)
        {
            // Locally set the singletons
            _events = events;
            _container = container;
        }
    }
}