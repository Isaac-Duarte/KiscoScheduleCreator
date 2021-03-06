﻿using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Shared.Models;
using KiscoSchedule.Shared.Util;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace KiscoSchedule.ViewModels
{
    class ShellViewModel : Conductor<object>, IHandle<SnackBarEventModel>, IHandle<HamburgerEventModel>, IHandle<ProgressEventModel>, IHandle<ScheduleEventModel>, IHandle<EmployeeEventModel>
    {
        private IEventAggregator _events;
        private SimpleContainer _container;
        private IDatabaseService _databaseService;
        private IUser _user;
        private SnackbarMessageQueue snackbarMessageQueue;
        private bool canHamburgerMenu;
        private bool leftDrawerOpen;
        private Visibility progressVisibility;

        public ShellViewModel(IEventAggregator events, SimpleContainer container, IDatabaseService databaseService, IUser user)
        {
            // Locally set the singletons
            _events = events;
            _container = container;
            _databaseService = databaseService;
            _user = user;

            _events.Subscribe(this);

            openDatabase();

            snackbarMessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue();
            snackbarMessageQueue.IgnoreDuplicate = true;

            ActivateItem(_container.GetInstance<LoginViewModel>());

            ProgressVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Opens/creates the database async
        /// </summary>
        private async void openDatabase()
        {
            _databaseService.CreateConnection(FileUtil.GetAppDataFolder() + @"\KiscoSchedule", "data.db");
            await _databaseService.OpenAsync();
        }

        /// <summary>
        /// Snackbar property
        /// </summary>
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get { return snackbarMessageQueue; }
            set
            {
                snackbarMessageQueue = value;
                NotifyOfPropertyChange(() => SnackbarMessageQueue);
            }
        }

        /// <summary>
        /// Enables/Disables the Hamburger menu
        /// </summary>
        public bool CanHamburgerMenu
        {
            get { return canHamburgerMenu; }
            set
            {
                canHamburgerMenu = value;
                NotifyOfPropertyChange(() => CanHamburgerMenu);
            }
        }

        /// <summary>
        /// Enables/Disable the Drawer
        /// </summary>
        public bool LeftDrawerOpen
        {
            get { return leftDrawerOpen; }
            set
            {
                leftDrawerOpen = value;
                NotifyOfPropertyChange(() => LeftDrawerOpen);
            }
        }

        /// <summary>
        /// Visibility of the progress bar
        /// </summary>
        public Visibility ProgressVisibility
        {
            get { return progressVisibility; }
            set
            {
                progressVisibility = value;
                NotifyOfPropertyChange(() => ProgressVisibility);
            }
        }

        /// <summary>
        /// Event to handle Snackbar notifications
        /// </summary>
        /// <param name="message"></param>
        public void Handle(SnackBarEventModel message)
        {
            SnackbarMessageQueue.Enqueue(message.Message);
        }

        /// <summary>
        /// Event to handle Snackbar notifications
        /// </summary>
        /// <param name="message"></param>
        public void Handle(HamburgerEventModel message)
        {
            CanHamburgerMenu = message.CanOpen;
        }

        /// <summary>
        /// Event to handle Progress Bar
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ProgressEventModel message)
        {
            ProgressVisibility = message.Visibility;
        }

        /// <summary>
        /// Event to handle changing views
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ScheduleEventModel message)
        {
            ActivateItem(_container.GetInstance<ScheduleViewModel>());
        }

        /// <summary>
        /// Event to handle changing views
        /// </summary>
        /// <param name="message"></param>
        public void Handle(EmployeeEventModel message)
        {
            ActivateItem(_container.GetInstance<EmployeeViewModel>());
        }

        /// <summary>
        /// Click event for hamburger
        /// </summary>
        public void ToggleHamburger()
        {
            LeftDrawerOpen = LeftDrawerOpen;
        }

        /// <summary>
        /// The about button handler
        /// </summary>
        public void About()
        {
            System.Diagnostics.Process.Start("https://github.com/Isaac-Duarte/KiscoScheduleCreator");
        }

        /// <summary>
        /// This will logout the client
        /// </summary>
        public void Logout()
        {
            ActivateItem(_container.GetInstance<LoginViewModel>());
            LeftDrawerOpen = false;
            CanHamburgerMenu = false;
            _databaseService.SetPassword("");
            _user.Hash = "";
            _user.Id = 0;
            _user.UserName = "";
        }

        /// <summary>
        /// Change the view to the schedule control
        /// </summary>
        public void EmployeesControl()
        {
            ActivateItem(_container.GetInstance<EmployeeViewModel>());
            LeftDrawerOpen = false;
        }

        /// <summary>
        /// Shcedule page
        /// </summary>
        public void ScheduleControl()
        {
            ActivateItem(_container.GetInstance<ScheduleViewModel>());
            LeftDrawerOpen = false;
        }

        /// <summary>
        /// Settings page
        /// </summary>
        public void SettingsControl()
        {
            ActivateItem(_container.GetInstance<SettingsViewModel>());
            LeftDrawerOpen = false;
        }
        /// <summary>
        /// Shifts page
        /// </summary>
        public void TemplateControl()
        {
            ActivateItem(_container.GetInstance<TemplateViewModel>());
            LeftDrawerOpen = false;
        }
    }
}