using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Core.Infrastructure.Utils
{
    public  class UIDispatcher
    {
        public static Dispatcher Dispatcher { get; set; } = Dispatcher.CurrentDispatcher;
    }
}
