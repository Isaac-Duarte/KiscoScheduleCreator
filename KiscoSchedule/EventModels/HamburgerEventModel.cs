using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.EventModels
{
    public class HamburgerEventModel
    {
        public bool CanOpen { get; set; }

        public HamburgerEventModel(bool canOpen)
        {
            CanOpen = canOpen;
        }
    }
}
