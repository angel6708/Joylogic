using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    public interface ISaveService
    {

        string SavingDesc { get;}
        bool ExecSave();
        bool IsInUiThread { get; set; }
    }
}
