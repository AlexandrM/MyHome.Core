using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHome.Shared
{

    public class ScheduleHourModel
    {
        public string ScheduleId { get; set; }

        public ScheduleModel Schedule { get; set; }

        public int DayOfWeek { get; set; }

        public int Hour { get; set; }

        public string ValueId { get; set; }

        public ElementItemEnumModel Value { get; set; }

        public string RawValue { get; set; }
    }

}