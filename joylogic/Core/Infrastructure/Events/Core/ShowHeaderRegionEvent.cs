using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Events
{

    public class ShowHeaderRegionEvent : PubSubEvent<OperationEventArgs<ShowHeaderRegionEvenArgs>>
    {
    }

    public class ShowHeaderRegionEvenArgs 
    {
        public bool IsVisible { get; set; }
        public bool IsShowSearch { get; set; }

        public string SearchHintText { get; set; }
        public string HeaderText { get; set; }
    }
}
