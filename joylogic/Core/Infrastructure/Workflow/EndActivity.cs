using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class EndActivity : Activity
    {

        private EndActivity(NodeKinds k)
            : base(k)
        {
        }

        public EndActivity()
            : this(NodeKinds.End)
        {

        } 

    }
}
