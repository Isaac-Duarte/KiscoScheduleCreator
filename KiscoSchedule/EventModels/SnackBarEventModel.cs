using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.EventModels
{
    class SnackBarEventModel
    {
        public string Message { get; set; }

        public SnackBarEventModel(string message)
        {
            Message = message;
        }
    }
}
