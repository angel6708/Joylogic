using Core.Infrastructure.Controls;
using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Events
{
    public class MainRegionNavigateEventArgs
    {
        public Intent Intent { get; set; }
    }
}
