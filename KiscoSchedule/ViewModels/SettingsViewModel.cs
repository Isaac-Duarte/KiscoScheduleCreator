using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.ViewModels
{
    class SettingsViewModel : Screen
    {
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;
        private Dictionary<string, ISetting> settings;
        private string twilioAccountSID;
        private string twilioAuthToken;
        private string twilioPhoneNumber;

        public SettingsViewModel(IDatabaseService databaseHelper, IEventAggregator events, IUser user)
        {
            _databaseService = databaseHelper;
            _events = events;
            _user = user;

            LoadSettings();
        }

        private async void LoadSettings()
        {
            settings = await _databaseService.GetSettingsAsync(_user);

            await validateSetting("ACCOUNT_SID");
            await validateSetting("AUTH_TOKEN");
            await validateSetting("PHONE_NUMBER");

            twilioAccountSID = settings["ACCOUNT_SID"].Value;
            twilioAuthToken = settings["AUTH_TOKEN"].Value;
            twilioPhoneNumber = settings["PHONE_NUMBER"].Value;
        }

        private async Task validateSetting(string key)
        {
            if (!settings.ContainsKey(key))
            {
                Setting setting = new Setting
                {
                    Key = key,
                    Value = ""
                };

                long id = await _databaseService.CreateSettingAsync(_user, setting);
                setting.Id = (int) id;
                settings[key] = setting;
            }
        }

        private async void updateSetting(string key, string value)
        {
            if (!settings.ContainsKey(key))
            {
                return;
            }

            ISetting setting = settings[key];
            setting.Value = value;
            await _databaseService.UpdateSettingAsync(setting);
        }

        public string TwilioAccountSID
        {
            get
            {
                return twilioAccountSID;
            }
            set
            {
                twilioAccountSID = value;
                NotifyOfPropertyChange(() => TwilioAccountSID);
                updateSetting("ACCOUNT_SID", value);
            }
        }

        public string TwilioAuthToken
        {
            get
            {
                return twilioAuthToken;
            }
            set
            {
                twilioAuthToken = value;
                NotifyOfPropertyChange(() => TwilioAuthToken);
                updateSetting("AUTH_TOKEN", value);
            }
        }

        public string TwilioPhoneNumber
        {
            get
            {
                return twilioPhoneNumber;
            }
            set
            {
                twilioPhoneNumber = value;
                NotifyOfPropertyChange(() => TwilioPhoneNumber);
                updateSetting("PHONE_NUMBER", value);
            }
        }
    }
}
