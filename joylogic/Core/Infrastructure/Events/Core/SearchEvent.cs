using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Events
{
    public class SearchEvent : PubSubEvent<OperationEventArgs<string>>
    {
    }
}
