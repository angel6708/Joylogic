using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Controls
{
    public interface ICaseView
    {
    }

    public interface ICaseView<CaseSessionI>: ICaseView where CaseSessionI : ICaseSession { }
}
