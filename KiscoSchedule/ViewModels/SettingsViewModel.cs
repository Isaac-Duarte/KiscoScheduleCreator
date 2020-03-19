using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.Shared.Models;
using KiscoSchedule.Shared.Enums;
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
        private Dictionary<SettingEnum, ISetting> settings;
        private string twilioAccountSID;
        private string twilioAuthToken;
        private string twilioPhoneNumber;
        private string textMessageReply;

        public SettingsViewModel(IDatabaseService databaseHelper, IEventAggregator events, IUser user)
        {
            _databaseService = databaseHelper;
            _events = events;
            _user = user;

            loadSettings();
        }

        /// <summary>
        /// Loads the settings from the database
        /// </summary>
        private async void loadSettings()
        {
            settings = await _databaseService.GetSettingsAsync(_user);

            await validateSetting(SettingEnum.ACCOUNT_SID);
            await validateSetting(SettingEnum.AUTH_TOKEN);
            await validateSetting(SettingEnum.PHONE_NUMBER);
            await validateSetting(SettingEnum.TEXT_MESSAGE);

            twilioAccountSID = settings[SettingEnum.ACCOUNT_SID].Value;
            twilioAuthToken = settings[SettingEnum.AUTH_TOKEN].Value;
            twilioPhoneNumber = settings[SettingEnum.PHONE_NUMBER].Value;
            textMessageReply = settings[SettingEnum.TEXT_MESSAGE].Value;
        }

        /// <summary>
        /// Validates a setting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task validateSetting(SettingEnum key)
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

        /// <summary>
        /// Upadtes a setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private async void updateSetting(SettingEnum key, string value)
        {
            if (!settings.ContainsKey(key))
            {
                return;
            }

            ISetting setting = settings[key];
            setting.Value = value;
            await _databaseService.UpdateSettingAsync(setting);
        }

        /// <summary>
        /// Account SID Setting
        /// </summary>
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
                updateSetting(SettingEnum.ACCOUNT_SID, value);
            }
        }

        /// <summary>
        /// Auth Token setting
        /// </summary>
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
                updateSetting(SettingEnum.AUTH_TOKEN, value);
            }
        }

        /// <summary>
        /// Phone number setting
        /// </summary>
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
                updateSetting(SettingEnum.PHONE_NUMBER, value);
            }
        }

        /// <summary>
        /// Phone number setting
        /// </summary>
        public string TextMessageReply
        {
            get
            {
                return textMessageReply;
            }
            set
            {
                textMessageReply = value;
                NotifyOfPropertyChange(() => TextMessageReply);
                updateSetting(SettingEnum.TEXT_MESSAGE, value);
            }
        }
    }
}
