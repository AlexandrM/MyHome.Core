using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHome.Shared
{
    public enum ElementItemType
    {
        Indicator = 1,
        Manage = 2,
    }

    public class ElementItemModel
    {
        public string Id { get; set; }

        public string ElementId { get; set; }
        public ElementModel Element { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ElementItemType Type { get; set; }

        public bool IsEnum { get; set; }

        public bool AllowSchedule { get; set; }

        public List<ElementItemValueModel> Values { get; set; }

        public List<ElementItemEnumModel> EnumValues { get; set; }

        public string ModeId { get; set; }
        public ScheduleModel Mode { get; set; }

        public bool NotCollectChanges { get; set; }

        public int RefreshTime { get; set; }
    }
}