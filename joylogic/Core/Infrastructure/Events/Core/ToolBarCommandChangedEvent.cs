using Core.Infrastructure.Controls;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Core.Infrastructure.Events
{
    public class ToolBarCommandChangedEvent : PubSubEvent<OperationEventArgs<IReadOnlyCollection<IconTextDelegateCommand<string>>>>
    {
    }
}
