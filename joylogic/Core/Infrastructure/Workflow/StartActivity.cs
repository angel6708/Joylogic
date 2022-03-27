using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    public class StartActivity : Activity
    {
        private StartActivity(NodeKinds k)
            : base(k)
        {
        }

        public StartActivity()
            : this(NodeKinds.Start)
        {

        }
    }
}
