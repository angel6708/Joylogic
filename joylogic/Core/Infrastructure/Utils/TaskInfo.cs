using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Utils
{
    public class TaskInfo
    {

        public bool IsFinished { get; set; }
        public Func<object, bool> Handler { get; set; }
        public object Param { get; set; }
        public string Name { get; set; }

        public bool Handled { get; set; }

       public bool Retry { get; set; }
        public int TryTimes { get; set; }
    }
}
