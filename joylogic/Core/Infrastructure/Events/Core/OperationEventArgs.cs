using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Events
{
    public class OperationEventArgs<T>
    {
        public T TargetModel { get; set; }

        public bool Handled { get; set; }
    }
}
