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
        private string twilioApiKey;

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
            
            if (!settings.Any(setting => setting.Key == "ApiKey"))
            {
                ISetting apiSetting = new Setting
                {
                    Key = "ApiKey",
                    Value = ""
                };

                await _databaseService.CreateSetting(_user, apiSetting);
            }

            TwilioApiKey = settings.Where(a => a.Key == "ApiKey").First().Value;
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

        public string TwilioApiKey
        {
            get
            {
                return twilioApiKey;
            }
            set
            {
                twilioApiKey = value;
                NotifyOfPropertyChange(() => TwilioApiKey);
                updateSetting("ApiKey", value);
            }
        }
    }
}
