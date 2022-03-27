 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    public interface IForeachSession
    {
        ForeachEnumerator ForeachEnumerator { get; set; }
    }
}
