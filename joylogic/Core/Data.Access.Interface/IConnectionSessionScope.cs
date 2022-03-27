using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Access.Interface
{
    public interface IConnectionSessionScope : IDisposable, IRegable
    {
        bool HasOpen { get; set; }
    }
}
