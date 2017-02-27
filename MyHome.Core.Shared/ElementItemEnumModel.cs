using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHome.Shared
{

    public class ElementItemEnumModel
    {
        public string Id { get; set; }
        public string ElementItemId { get; set; }
        public ElementItemModel ElementItem { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }
    }

}