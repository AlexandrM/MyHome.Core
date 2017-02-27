using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHome.Shared
{

    public class ElementModel
    {
        public string Id { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public List<ElementItemModel> Items { get; set; }        
    }
}