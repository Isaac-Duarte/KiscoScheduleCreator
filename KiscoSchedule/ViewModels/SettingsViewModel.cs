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
        private List<ISetting> settings;
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
            
            if (!settings.Any(setting => setting.Key == "ACCOUNT_SID"))
            {
                ISetting apiSetting = new Setting
                {
                    Key = "ACCOUNT_SID",
                    Value = ""
                };

                await _databaseService.CreateSetting(_user, apiSetting);
                settings.Add(apiSetting);
            }

            if (!settings.Any(setting => setting.Key == "AUTH_TOKEN"))
            {
                ISetting apiSetting = new Setting
                {
                    Key = "AUTH_TOKEN",
                    Value = ""
                };

                await _databaseService.CreateSetting(_user, apiSetting);
                settings.Add(apiSetting);
            }

            TwilioAccountSID = settings.Where(a => a.Key == "ACCOUNT_SID").First().Value;
            TwilioAuthToken = settings.Where(a => a.Key == "AUTH_TOKEN").First().Value;
        }

        private async void updateSetting(string key, string value)
        {
            if (!settings.Any(a => a.Key == key))
            {
                return;
            }

            ISetting setting = settings.Where(a => a.Key == key).First();

            setting.Value = value;
            await _databaseService.UpdateSetting(setting);
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
