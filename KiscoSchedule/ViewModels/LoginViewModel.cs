using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.EventModels;
using KiscoSchedule.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.ViewModels
{
    class LoginViewModel : Screen
    {
        private string _username;
        private string _password;
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;

        /// <summary>
        /// Constuctor for LoginViewModel
        /// </summary>
        public LoginViewModel(IDatabaseService databaseService, IEventAggregator events, IUser user)
        {
            _databaseService = databaseService;
            _events = events;
            _user = user;
        }

        /// <summary>
        /// Username from the login form
        /// </summary>
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanLogin);
                NotifyOfPropertyChange(() => CanRegister);
            }
        }

        /// <summary>
        /// Password from the login form
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogin);
                NotifyOfPropertyChange(() => CanRegister);
            }
        }

        /// <summary>
        /// Returns weather the client can login or not
        /// </summary>
        public bool CanLogin
        {
            get
            {
                bool output = false;

                if (Username?.Length > 0 && Password?.Length >= 8)
                {
                    output = true;
                }

                return output;
            }
        }

        /// <summary>
        /// Returns weather the client can register or not
        /// </summary>
        public bool CanRegister
        {
            get
            {
                bool output = false;

                if (Username?.Length > 0 && Password?.Length >= 8)
                {
                    output = true;
                }

                return output;
            }
        }

        /// <summary>
        /// This will attempt to login given the login credentials
        /// </summary>
        public async void Login()
        {
            string passwordHash = CryptoService.Hash(Password);
            User selectedUser = await _databaseService.GetUserByUsernameAsync(Username);

            if (selectedUser.Hash == null || selectedUser.Hash != passwordHash)
            {
                _events.PublishOnUIThread(new SnackBarEventModel("The username or password was incorrect!"));
                return;
            }

            _databaseService.SetPassword(Password);

            _user.Hash = selectedUser.Hash;
            _user.Id = selectedUser.Id;
            _user.UserName = selectedUser.UserName;

            _events.PublishOnUIThread(new SnackBarEventModel("Success! Logging in!"));
            _events.PublishOnUIThread(new EmployeeEventModel());
        }

        /// <summary>
        /// This will attempt to register given the login credentials
        /// </summary>
        public async void Register()
        {
            string hash = CryptoService.Hash(Password);

            try
            {
                await _databaseService.CreateUserAsync(new User
                {
                    UserName = Username,
                    Hash = hash
                });

                _events.PublishOnUIThread(new SnackBarEventModel($"Created user {Username}!"));

                Username = "";
                Password = "";

            }
            catch (Exception e)
            {
                if (e.HResult == -2147473489)
                {
                    _events.PublishOnUIThread(new SnackBarEventModel($"There is already an existing user named {Username}!"));
                }
            }
        }
    }
}
