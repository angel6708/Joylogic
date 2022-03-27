using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Core.Infrastructure.Workflow
{
    [Serializable]
    public delegate void ActivityEvent();


    [DataContract]
    public class ActivityEventHandler
    {
       
        [DataMember]
        public ActivityEvent ActivityEvent { get; set; }
    }
}
