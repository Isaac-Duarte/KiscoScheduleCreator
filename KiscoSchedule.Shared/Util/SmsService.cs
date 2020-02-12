using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace KiscoSchedule.Shared.Util
{
    public class SmsService
    {
        private string accountSid;
        private string authToken;
        private string phoneNumber;

        /// <summary>
        /// Constructor for SmsService
        /// </summary>
        /// <param name="accountSid"></param>
        /// <param name="authToken"></param>
        /// <param name="phoneNumber"></param>
        public SmsService(string accountSid, string authToken, string phoneNumber)
        {
            this.accountSid = accountSid;
            this.authToken = authToken;
            this.phoneNumber = phoneNumber;
        }

        /// <summary>
        /// Sends a text message
        /// </summary>
        /// <param name="number"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async void SendMessage(string number, string message)
        {
            TwilioClient.Init(accountSid, authToken);

            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(phoneNumber),
                to: new Twilio.Types.PhoneNumber(number)
            );
        }
    }
}
