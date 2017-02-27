using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHome.Shared
{

    public class ElementItemValueModel
    {
        public DateTime DateTime { get; set; }

        public DateTime Updated { get; set; }

        public string ElementItemId { get; set; }
        public ElementItemModel ElementItem { get; set; }

        public string ValueId { get; set; }
        public ElementItemEnumModel Value { get; set; }

        public string RawValue { get; set; }
    }

}