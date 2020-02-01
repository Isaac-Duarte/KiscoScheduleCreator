using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KiscoSchedule.EventModels
{
    class ProgressEventModel
    {
        public Visibility Visibility;

        public ProgressEventModel(Visibility visibility)
        {
            Visibility = visibility;
        }
    }
}
