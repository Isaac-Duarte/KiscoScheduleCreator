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

            if (!settings.ContainsKey("ACCOUNT_SID"))
            {
                settings["ACCOUNT_SID"] = new Setting
                {
                    Key = "ACCOUNT_SID",
                    Value = ""
                };
            }

            if (!settings.ContainsKey("AUTH_TOKEN"))
            {
                settings["AUTH_TOKEN"] = new Setting
                {
                    Key = "AUTH_TOKEN",
                    Value = ""
                };
            }

            TwilioAccountSID = settings["ACCOUNT_SID"].Value;
            TwilioAuthToken = settings["AUTH_TOKEN"].Value;
        }

        private async void updateSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
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
    }
}
